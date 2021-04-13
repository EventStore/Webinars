using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Hotel.Bookings {
    public static class Program {
        public static async Task<int> Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try {
                await CreateHostBuilder(args).Build().RunAsync();
                return 0;
            }
            catch (Exception ex) {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally {
                Log.CloseAndFlush();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    webBuilder => {
                        webBuilder.UseSerilog();
                        webBuilder.UseStartup<Startup>();
                    }
                );
    }
}
