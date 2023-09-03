using Discount.Infrastructure.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discount.Infrastructure.Extensions;

public static class DbExtension
{
    public static IHost MigrateDatabase<TContext>(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
            var databaseService = scope.ServiceProvider.GetRequiredService<Database>();
            var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            
            try
            {
                logger.LogInformation("Discount DB Migration started");
                migrationService.ListMigrations();
                migrationService.MigrateUp();
                // migrationService.MigrateDown(202309010001);
                logger.LogInformation("Discount DB Migration completed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return host;
    }
}