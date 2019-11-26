using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using(var scope = host.Services.CreateScope())
            {
                // Get all services
                var services = scope.ServiceProvider;
                try
                {
                    // Get DataContext from the services, and apply migrations 
                    // Also creates the database if it doesn't already exist
                    var context = services.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    // If an exception occured, get the loger from the services,
                    // and log the error to the terminal
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                }

                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
