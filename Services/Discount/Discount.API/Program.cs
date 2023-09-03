using System.Reflection;
using Discount.API.Services;
using Discount.Application.Handlers;
using Discount.Application.Mapper;
using Discount.Core.Repositories;
using Discount.Infrastructure.Context;
using Discount.Infrastructure.Repositories;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Migrations;
using FluentMigrator.Runner;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<Database>();

builder.Services.AddLogging(c => c.AddFluentMigratorConsole())
    .AddFluentMigratorCore()
    .ConfigureRunner(c => c.AddPostgres()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

builder.Services.AddGrpc();

builder.Services.AddMediatR(cfg=>
    cfg.RegisterServicesFromAssemblies(typeof(CreateDiscountHandler)
        .GetTypeInfo().Assembly));
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddAutoMapper(typeof(DiscountProfile));

var app = builder.Build();

app.MigrateDatabase<Program>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<DiscountService>();
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
    });
});

// Configure the HTTP request pipeline.
// app.MapGrpcService<DiscountService>();
// app.MapGet("/",
//     () =>
//         "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();