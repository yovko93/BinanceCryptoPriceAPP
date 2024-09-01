using Application.Interfaces;
using Application.Services;
using BinanceCryptoPriceAPI.Infrastructure.Middleware;
using Data;
using Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with PostgreSQL provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.CommandTimeout(300)));

// Register the PriceDataCollector and WebSocket Service
builder.Services.AddScoped<PriceDataCollector>();
builder.Services.AddHostedService<BinanceWebSocketService>();

builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

builder.Services.AddScoped<IPriceService, PriceService>();

builder.Services.AddMemoryCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await DatabaseInit.InitializeDatabaseAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Binance Crypto Price API V1");
    });
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
