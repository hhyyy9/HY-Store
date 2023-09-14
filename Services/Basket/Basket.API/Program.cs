using System.Reflection;
using Basket.Application.Handlers;
using Basket.Application.Mappers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using MassTransit;
using MassTransit.MultiBus;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Basket.API",
            Version = "v1"
        }
    ));
builder.Services.AddApiVersioning();
//Redis Settings
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["CacheSettings:ConnectionString"];
});

builder.Services.AddMediatR(cfg=>
    cfg.RegisterServicesFromAssemblies(typeof(CreateShoppingCartCommandHandler)
        .GetTypeInfo().Assembly));
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(BasketMappingProfile));
builder.Services.AddHealthChecks()
    .AddRedis(builder.
            Configuration["CacheSettings:ConnectionString"], 
        "Redis Health",HealthStatus.Degraded);
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});
builder.Services.AddMassTransitHostedService();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c=>
        c.SwaggerEndpoint("/swagger/v1/swagger.json","Basket.API v1"));
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});
app.MapControllers();

app.Run();