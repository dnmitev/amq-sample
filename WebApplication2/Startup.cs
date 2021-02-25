using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

using MassTransit;
using MassTransit.ActiveMqTransport;

using MessageContracts;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using WebApplication2.HealthChecks;

namespace WebApplication2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddMassTransit(x =>
            {
                var serviceAddress = new Uri($"activemq://{Environment.GetEnvironmentVariable("MQ_HOSTNAME")}/{Environment.GetEnvironmentVariable("MQ_QUEUE_NAME")}");

                x.AddBus(ctx => Bus.Factory.CreateUsingActiveMq(cfg =>
                 {
                     cfg.Host($"{Environment.GetEnvironmentVariable("MQ_HOSTNAME")}",
                         h =>
                         {
                             h.Username(Environment.GetEnvironmentVariable("MQ_USERNAME"));
                             h.Password(Environment.GetEnvironmentVariable("MQ_PASSWORD"));
                         });

                     cfg.UseHealthCheck(ctx);
                 }));

                x.AddRequestClient<SubmitOrder>(serviceAddress);
                x.AddRequestClient<SubmitNewOrder>(new Uri($"activemq://{Environment.GetEnvironmentVariable("MQ_HOSTNAME")}/order-count"));
            });

            services.AddMassTransitHostedService();
            services.AddRazorPages();

            var shouldAddFaillingHealthCheck = bool.Parse(Environment.GetEnvironmentVariable("ENABLE_FAILING_HEALTH_CHECK"));

            if (shouldAddFaillingHealthCheck)
            {
                services.AddHealthChecks()
                   .AddCheck<FailingMemoryCacheHealthCheck>("failing_health_check")
                   .AddCheck<MemoryCacheHealthCheck>("custom_health_check");
            }
            else
            {
                services.AddHealthChecks()
                   .AddCheck<MemoryCacheHealthCheck>("custom_health_check");
            }
           

            MemoryCache.Default["access_token"] = System.Guid.NewGuid();

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(10);
                options.Predicate = (check) => check.Tags.Contains("ready");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions());
            });
        }
    }
}
