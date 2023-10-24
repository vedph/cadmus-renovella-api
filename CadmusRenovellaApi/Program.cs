using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Cadmus.Api.Services.Seeding;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CadmusRenovellaApi
{
    /// <summary>
    /// Program.
    /// </summary>
    public static class Program
    {
        private static void DumpEnvironmentVars()
        {
            Console.WriteLine("ENVIRONMENT VARIABLES:");
            IDictionary dct = Environment.GetEnvironmentVariables();
            List<string> keys = new();
            var enumerator = dct.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(((DictionaryEntry)enumerator.Current).Key.ToString()!);
            }

            foreach (string key in keys.OrderBy(s => s))
                Console.WriteLine($"{key} = {dct[key]}");
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static async Task<int> Main(string[] args)
        {
            try
            {
                Log.Information("Starting Cadmus Renovella API host");
                DumpEnvironmentVars();

                // this is the place for seeding:
                // see https://stackoverflow.com/questions/45148389/how-to-seed-in-entity-framework-core-2
                // and https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/?view=aspnetcore-2.1#move-database-initialization-code
                var host = await CreateHostBuilder(args)
                    .UseSerilog((hostingContext, loggerConfiguration) =>
                    {
                        string cs = hostingContext.Configuration
                            .GetConnectionString("Log")!;
                        string? maxSize = hostingContext.Configuration
                            ["Serilog:MaxMbSize"] ?? "10";
                        string? logPath = hostingContext.Configuration["Serilog:LogPath"];
#if DEBUG
                        if (string.IsNullOrEmpty(logPath)) logPath = "cadmus-log.txt";
#endif
                        if (!string.IsNullOrEmpty(logPath))
                        {
                            loggerConfiguration
                                .ReadFrom.Configuration(hostingContext.Configuration)
                                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                                .WriteTo.MongoDBCapped(cs,
                                    cappedMaxSizeMb: !string.IsNullOrEmpty(maxSize) &&
                                        int.TryParse(maxSize, out int n) && n > 0 ? n : 10);
                        }
                        else
                        {
                            loggerConfiguration
                                .ReadFrom.Configuration(hostingContext.Configuration)
                                .WriteTo.MongoDBCapped(cs,
                                    cappedMaxSizeMb: !string.IsNullOrEmpty(maxSize) &&
                                        int.TryParse(maxSize, out int n) && n > 0 ? n : 10);
                        }
                    })
                    .Build()
                    .SeedAsync(); // see Services/HostSeedExtension

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Cadmus Renovella API host terminated unexpectedly");
                Debug.WriteLine(ex.ToString());
                Console.WriteLine(ex.ToString());
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
