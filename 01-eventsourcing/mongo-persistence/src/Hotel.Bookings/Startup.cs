using System.Threading.Tasks;
using EventSourcing.Lib;
using EventStore.Client;
using Hotel.Bookings.Application.Bookings;
using Hotel.Bookings.Domain;
using Hotel.Bookings.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;

namespace Hotel.Bookings {
    public class Startup {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            // Infrastructure
            services.AddSingleton(
                ConfigureMongo(
                    Configuration["MongoDb:ConnectionString"],
                    Configuration["MongoDb:Database"]
                )
            );
            services.AddSingleton<IAggregateStore, MongoAggregateStore>();

            // Application
            services.AddSingleton<BookingsCommandService>();

            // Domain services
            services.AddSingleton<Services.IsRoomAvailable>(((id, period) => new ValueTask<bool>(true)));
            services.AddSingleton<Services.ConvertCurrency>((from, currency) => new Money(from.Amount * 2, currency));

            // API
            services.AddControllers();

            services.AddSwaggerGen(
                c
                    => c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo {
                            Title = "Event Sourcing", Version = "v0.1"
                        }
                    )
            );
        }

        public void Configure(IApplicationBuilder app) {
            app.UseSwagger();

            app.UseSwaggerUI(
                c => c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Event Sourcing v0.1"
                )
            );
            app.UseDeveloperExceptionPage();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        static IMongoDatabase ConfigureMongo(string connectionString, string database) {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            return new MongoClient(settings).GetDatabase(database);
        }

        static EventStoreClient ConfigureEventStore(string connectionString, ILoggerFactory loggerFactory) {
            // AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // services.AddSingleton(
            //     ctx
            //         => ConfigureEventStore(
            //             Configuration["EventStore:ConnectionString"],
            //             ctx.GetService<ILoggerFactory>()
            //         )
            // );

            var settings = EventStoreClientSettings.Create(connectionString);
            settings.ConnectionName = "bookingApp";
            settings.LoggerFactory  = loggerFactory;
            return new EventStoreClient(settings);
        }
    }
}
