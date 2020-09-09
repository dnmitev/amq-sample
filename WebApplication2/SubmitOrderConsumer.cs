using System.Threading.Tasks;

using MassTransit;

using MessageContracts;

namespace WebApplication2
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            await context
                .RespondAsync<OrderAccepted>(new { OrderId = context.Message.OrderId })
                .ConfigureAwait(false);
        }
    }
}