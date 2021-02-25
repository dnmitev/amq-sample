using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks
{
    public class HttpHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            int statusCode = 200;
            if (DateTime.Now.Minute % 2 == 0)
            {
                statusCode = 401;
            }
            var client = _httpClientFactory.CreateClient();

            var res = await client
                .GetAsync($"https://httpbin.org/status/{statusCode}")
                .ConfigureAwait(false);

            if (!res.IsSuccessStatusCode)
            {
                return HealthCheckResult.Unhealthy($"Request failed with status code: {res.StatusCode}");
            }

            return HealthCheckResult.Healthy("Httpbin successfully called.");
        }
    }
}
