using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using MessageContracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IRequestClient<SubmitOrder> _client;

        public WeatherForecastController(IRequestClient<SubmitOrder> requestClient)
        {
            _client = requestClient;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var response = await _client
                .GetResponse<OrderAccepted>(new { OrderId = $"OrderID#{Guid.NewGuid().ToString("N")}" })
                .ConfigureAwait(false);

            return response.Message.OrderId;
        }
    }
}
