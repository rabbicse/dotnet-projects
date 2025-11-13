using SSEExample.Services;
using System.Net.ServerSentEvents;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
}

builder.Services.AddSingleton<StockService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowFrontend");
}

app.MapGet("/stocks", (StockService stockService, CancellationToken ct) =>
{
    return TypedResults.ServerSentEvents(
        stockService.GenerateStockPrices(ct),
        eventType: "stockUpdate"
    );
});

app.MapGet("/stocks2", (
    StockService stockService,
    HttpRequest httpRequest,
    CancellationToken ct) =>
{
    // 1. Read Last-Event-ID (if any)
    var lastEventId = httpRequest.Headers.TryGetValue("Last-Event-ID", out var id)
        ? id.ToString()
        : null;

    // 2. Optionally log or handle resume logic
    if (!string.IsNullOrEmpty(lastEventId))
    {
        app.Logger.LogInformation("Reconnected, client last saw ID {LastId}", lastEventId);
    }

    // 3. Stream SSE with IDs and retry
    var stream = stockService.GenerateStockPricesSince(lastEventId, ct)
        .Select(evt =>
        {
            var sseItem = new SseItem<StockPriceEvent>(evt, "stockUpdate")
            {
                EventId = evt.Id
            };

            return sseItem;
        });

    return TypedResults.ServerSentEvents(
        stream,
        eventType: "stockUpdate"
    );
});

app.Run();
