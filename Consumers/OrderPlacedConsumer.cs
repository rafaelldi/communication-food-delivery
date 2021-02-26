using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using static CommunicationFoodDelivery.Contracts.Events;

namespace CommunicationFoodDelivery.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlaced>
    {
        private readonly ILogger<OrderPlacedConsumer> _logger;

        public OrderPlacedConsumer(ILogger<OrderPlacedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderPlaced> context)
        {
            await Task.Delay(500);

            _logger.LogInformation("Order with id = {id} and details = {details} was placed",
                context.Message.OrderId.ToString(), context.Message.OrderDetails);
            _logger.LogInformation("Sending notification to the manager...");
        }
    }
}