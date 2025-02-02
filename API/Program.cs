using Knab.Assignment.API.Extensions;
using Knab.Assignment.API.Models;
using Knab.Assignment.API.Providers;
using Knab.Assignment.API.Services;
using Knab.Assignment.Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IJWTTokenProvider, JWTTokenProvider>();
builder.Services.AddSingleton<ICryptoService, CryptoService>();
builder.Services.AddSingleton<ICryptoProvider, CoinMarketCapProvider>();
builder.Services.AddSingleton<ICryptoProvider, ExchangeRatesProvider>();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddHttpClient("ExchangeRates", httpClient =>
{
    var options = new ExchangeRatesOptions();
    builder.Configuration.GetSection(ExchangeRatesOptions.Path).Bind(options);
    httpClient.BaseAddress = new Uri(options.BaseUrl);
});
builder.Services.AddHttpClient("CoinMarketCap", httpClient =>
{
    var options = new CoinMarketCapOptions();
    builder.Configuration.GetSection(CoinMarketCapOptions.Path).Bind(options);
    httpClient.BaseAddress = new Uri(options.BaseUrl);
    httpClient.DefaultRequestHeaders.Add(options.ApiKeyHeader, options.ApiKey);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
