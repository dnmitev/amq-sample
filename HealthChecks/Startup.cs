using HealthChecks.DB;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Npgsql;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace HealthChecks
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
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost",
                Database = Environment.GetEnvironmentVariable("DB_NAME") ?? "OBOL_Aggregator",
                Username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "postgres",
                Password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "secret",
            };

            services.AddControllers();
            services.AddLogging();
            services
                .AddHealthChecks()
                .AddDbContextCheck<PostContext>()
                .AddCheck<HttpHealthCheck>("http_health_check")
                .AddCheck<MemoryHealthCheck>("memory_health_check");

            services.AddHttpClient();

            services.AddDbContext<PostContext>((provider, options) => options
                     .UseNpgsql(connectionStringBuilder.ConnectionString));

            services.AddSwaggerDocument();


            MemoryCache.Default["trusso_token"] = System.Guid.NewGuid().GetHashCode();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var ctx = serviceScope.ServiceProvider.GetRequiredService<PostContext>();
                ctx.Database.EnsureCreated();
            }

            app.UseRouting();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseAuthorization();

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
