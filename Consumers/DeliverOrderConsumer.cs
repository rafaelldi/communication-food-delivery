using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using static CommunicationFoodDelivery.Contracts.Commands;
using static CommunicationFoodDelivery.Contracts.Events;

namespace CommunicationFoodDelivery.Consumers
{
    public class DeliverOrderConsumer : IConsumer<DeliverOrder>
    {
        private readonly ILogger<DeliverOrderConsumer> _logger;

        public DeliverOrderConsumer(ILogger<DeliverOrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeliverOrder> context)
        {
            await Task.Delay(500);

            var orderId = context.Message.OrderId;
            _logger.LogInformation("Order with id = {id} was delivered", orderId.ToString());
            await context.Publish(new OrderDelivered {OrderId = orderId});
        }
    }
}