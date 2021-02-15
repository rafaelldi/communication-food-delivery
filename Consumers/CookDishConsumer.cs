using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using static CommunicationFoodDelivery.Contracts.Commands;
using static CommunicationFoodDelivery.Contracts.Events;

namespace CommunicationFoodDelivery.Consumers
{
    public class CookDishConsumer : IConsumer<CookDish>
    {
        private readonly ILogger<CookDishConsumer> _logger;

        public CookDishConsumer(ILogger<CookDishConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CookDish> context)
        {
            var orderId = context.Message.OrderId;
            _logger.LogInformation("Dish for order with id = {id} was cooked", orderId.ToString());
            await context.Publish(new DishCooked {OrderId = orderId});
        }
    }
}