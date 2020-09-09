using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;
using MassTransit.ActiveMqTransport;

using MessageContracts;

using Microsoft.Extensions.Hosting;

namespace Sample_RequestResponse
{
    public class MessageQueueService : BackgroundService
    {
        private readonly IBusControl _bus;

        public MessageQueueService()
        {
            _bus = Bus.Factory.CreateUsingActiveMq(cfg =>
            {
                cfg.Host(Environment.GetEnvironmentVariable("MQ_HOSTNAME"), h =>
                {
                    h.Username(Environment.GetEnvironmentVariable("MQ_USERNAME"));
                    h.Password(Environment.GetEnvironmentVariable("MQ_PASSWORD"));
                });

                cfg.ReceiveEndpoint("order-service", ep =>
                {
                    ep.Handler<SubmitOrder>(context => context.RespondAsync<OrderAccepted>(new
                    {
                        context.Message.OrderId
                    }));
                });
            });
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _bus.StartAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(base.StopAsync(cancellationToken), _bus.StopAsync());
        }
    }
}
