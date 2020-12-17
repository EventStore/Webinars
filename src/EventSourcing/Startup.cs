using System;
using EventSourcing.Infrastructure;
using EventSourcing.Lib;
using EventStore.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;

namespace EventSourcing
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            // EventTypeMapper.MapEventTypes();

            services.AddControllers();

            services.AddSingleton(
                ctx
                    => ConfigureEventStore(
                        Configuration["EventStore:ConnectionString"],
                        ctx.GetService<ILoggerFactory>()
                    )
            );

            services.AddSingleton<IAggregateStore, AggregateStore>();

            services.AddSwaggerGen(
                c
                    => c.SwaggerDoc("v1", new OpenApiInfo {Title = "Event Sourcing", Version = "v0.1"})
            );
            
        }

        public void Configure(IApplicationBuilder app)
        {
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
        
        static EventStoreClient ConfigureEventStore(string connectionString, ILoggerFactory loggerFactory) {
            var settings = EventStoreClientSettings.Create(connectionString);
            settings.ConnectionName = "bookingApp";
            settings.LoggerFactory  = loggerFactory;
            return new EventStoreClient(settings);
        }
    }
}
