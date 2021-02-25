using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks
{
    public class MemoryHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            string token = string.Empty;

            if (DateTime.Now.Minute % 2 == 0)
            {
                token = MemoryCache.Default["failing_trusso_token"]?.ToString();
            }
            else
            {
                token = MemoryCache.Default["trusso_token"]?.ToString();
            }

            if (string.IsNullOrEmpty(token.ToString()))
            {
                return Task.FromResult(HealthCheckResult.Degraded("Token not found in MemoryCache"));
            }

            return Task.FromResult(HealthCheckResult.Healthy("Token obtained from MemoryCache"));
        }
    }
}
