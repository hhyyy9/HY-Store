using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Application.Mappers;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;
using Ordering.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();

// builder.Services.AddDbContext<OrderContext>(options => options.UseSqlServer(
//     builder.Configuration.GetConnectionString("OrderingConnectionString"),  
//     b => b.MigrationsAssembly("Ordering.API")
//     ));
// builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
// builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//builder.WebHost.UseStartup<Program>();
builder.Services.AddInfraServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddApiVersioning();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<BasketOrderingConsumer>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "Ordering.API", Version = "v1"});
});
builder.Services.AddHealthChecks().Services.AddDbContext<OrderContext>();

builder.Services.AddMassTransit(config =>
{
    //Mark this as consumer
    config.AddConsumer<BasketOrderingConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        //provide the queue name with consumer settings
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
        });
    });
});
builder.Services.AddMassTransitHostedService();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
}

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});

// using (var scope = app.Services.CreateScope())
// {
//     var _db = scope.ServiceProvider.GetRequiredService<OrderContext>();
//     if (_db.Database.GetPendingMigrations().Count() > 0)
//     {
//         _db.Database.Migrate();
//     }
// }

app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger).Wait();
});


app.Run();