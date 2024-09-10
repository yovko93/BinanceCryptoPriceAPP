using BinanceCryptoPriceAPI.Infrastructure.Extensions;
using BinanceCryptoPriceAPI.Infrastructure.Middleware;
using Data;

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddAppDbContext(builder.Configuration);
builder.Services.AddAppServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppSwagger();

builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
