using BitFlow.Web.Components;
using BitFlow.Web.Native;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

#if DEBUG
builder.Services.AddScoped(_ =>
    new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5286") // windows
    });
#endif

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

static string EscapeJson(string text)
{
    return text
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\n", "\\n")
        .Replace("\r", "\\r")
        .Replace("\t", "\\t");
}

app.MapGet("/api/rewrite", (string expr) =>
{
    try
    {
        using var ctx = new BitFlowContext();

        var parsedId = ctx.Parse(expr);
        //var parsed = ctx.ToText(parsedId);
        var parsed = EscapeJson(ctx.ToLatex(parsedId));

        var rewrittenId = ctx.Rewrite(parsedId);
        //var rewritten = ctx.ToText(rewrittenId);
        var rewritten = EscapeJson(ctx.ToLatex(rewrittenId));

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
