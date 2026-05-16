using BitFlow.Api.Native;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapGet("/parse", (string expr) =>
{
    using var context = new BitFlowContext();

    var id = context.Parse(expr);
    var text = context.ToText(id);

    return Results.Ok(new
    {
        input = expr,
        exprId = id,
        output = text
    });
});

app.MapGet("/rewrite", (string expr) =>
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