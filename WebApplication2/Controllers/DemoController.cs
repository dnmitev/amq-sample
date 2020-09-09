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
    public class DemoController : ControllerBase
    {
        private readonly IRequestClient<SubmitOrder> _submitOrderClient;
        private readonly IRequestClient<SubmitNewOrder> _submitNewOrderClient;

        public DemoController(IRequestClient<SubmitOrder> requestClient, IRequestClient<SubmitNewOrder> newOrderClient)
        {
            _submitOrderClient = requestClient;
            _submitNewOrderClient = newOrderClient;
        }

        [HttpGet]
        [Route("order_id")]
        public async Task<string> Get()
        {
            var response = await _submitOrderClient
                .GetResponse<OrderAccepted>(new { OrderId = $"OrderID#{Guid.NewGuid():N}" })
                .ConfigureAwait(false);

            return response.Message.OrderId;
        }

        [HttpGet]
        [Route("order_count")]
        public async Task<string> GetCount()
        {
            var response = await _submitNewOrderClient
                .GetResponse<OrderAccepted>(new { Count = 2 })
                .ConfigureAwait(false);

            return response.Message.OrderId;
        }
    }
}
