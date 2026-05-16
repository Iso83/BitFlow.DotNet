using BitFlow.Web.Native;
using BitFlow.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(_ =>
    new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5286") // use 8080 for docker
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/rewrite", (string expr) =>
{
    try
    {
        using var ctx = new BitFlowContext();

        var parsedId = ctx.Parse(expr);
        var parsed = ctx.ToText(parsedId);

        var rewrittenId = ctx.Rewrite(parsedId);
        var rewritten = ctx.ToText(rewrittenId);

        var traceJson = ctx.GetTraceJson();

        return Results.Content(
$$"""
{
    "input": "{{expr}}",
    "parsed": "{{parsed}}",
    "rewritten": "{{rewritten}}",
    "trace": {{traceJson}}
}
""",
            "application/json"
        );
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            error = ex.Message
        });
    }
});

app.Run();
