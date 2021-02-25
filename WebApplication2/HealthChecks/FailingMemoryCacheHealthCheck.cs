using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication2.HealthChecks
{
    public class FailingMemoryCacheHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var token = MemoryCache.Default["failing_access_token"];

            if (string.IsNullOrEmpty(token?.ToString()))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Missing token in Memory cache"));
            }

            return Task.FromResult(HealthCheckResult.Healthy("Token available in memory cache"));
        }
    }
}
