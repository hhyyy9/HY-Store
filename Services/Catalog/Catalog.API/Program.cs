using System.Net;
using Catalog.Application.Mappers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
    c.SwaggerDoc("v1",
    new OpenApiInfo
    {
        Title = "Catalog.API",
        Version = "v1"
    }
    ));
builder.Services.AddApiVersioning();
builder.Services.AddHealthChecks()
    .AddMongoDb(builder.
            Configuration["DatabaseSettings:ConnectionString"], 
        "Catalog Mongo Db Health Check",HealthStatus.Degraded);
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));
// builder.Services.AddMediatR(cfg=>
//     cfg.RegisterServicesFromAssemblies(typeof(CreateProductHandler)
//         .GetTypeInfo().Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, ProductRepository>();
builder.Services.AddScoped<ITypesRepository, ProductRepository>();

// builder.Services.AddControllers();
//Identity Server changes
#if DEBUG
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#endif
var userPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
builder.Services.AddControllers(config =>
{
    config.Filters.Add(new AuthorizeFilter(userPolicy));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:9009";
        options.MetadataAddress = "https://localhost:9009/.well-known/openid-configuration";
        options.Audience = "Catalog";
        // options.RequireHttpsMetadata = false;
        // options.SaveToken = true;
        
        // options.Configuration = new OpenIdConnectConfiguration();
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
        options.BackchannelHttpHandler = handler;
    });
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("CanRead", policy => policy.RequireClaim("scope", "catalogapi.read"));
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c=>
        c.SwaggerEndpoint("/swagger/v1/swagger.json","Catalog.API v1"));
    app.UseDeveloperExceptionPage();
    IdentityModelEventSource.ShowPII = true;
}

app.UseRouting();
app.UseAuthentication();
app.UseStaticFiles();
app.UseHttpsRedirection();

//app.UseAuthorization();
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