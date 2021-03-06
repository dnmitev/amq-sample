using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using MessageContracts;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAppRmq
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
                var serviceAddress = new Uri($"rabbitmq://{Environment.GetEnvironmentVariable("MQ_HOSTNAME")}/{Environment.GetEnvironmentVariable("MQ_QUEUE_NAME")}");

                x.AddBus(ctx => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host($"{Environment.GetEnvironmentVariable("MQ_HOSTNAME")}",
                        h =>
                        {
                            h.Username(Environment.GetEnvironmentVariable("MQ_USERNAME"));
                            h.Password(Environment.GetEnvironmentVariable("MQ_PASSWORD"));
                        });
                }));

                x.AddRequestClient<SubmitOrder>(serviceAddress);
                x.AddRequestClient<SubmitNewOrder>(new Uri($"rabbitmq://{Environment.GetEnvironmentVariable("MQ_HOSTNAME")}/order-count"));
            });

            services.AddMassTransitHostedService();
            services.AddRazorPages();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
