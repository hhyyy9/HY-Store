using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true)
    .Build();

builder.Services.AddOcelot(configuration)
    .AddCacheManager(o => o.WithDictionaryHandle());


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello Ocelot"); });
});
await app.UseOcelot();

app.Run();