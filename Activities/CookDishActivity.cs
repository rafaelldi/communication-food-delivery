using System.Threading.Tasks;
using CommunicationFoodDelivery.Contracts;
using MassTransit;
using MassTransit.Courier;

namespace CommunicationFoodDelivery.Activities
{
    public class CookDishActivity : IExecuteActivity<CookDishArgument>
    {
        private readonly IBus _bus;

        public CookDishActivity(IBus bus)
        {
            _bus = bus;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<CookDishArgument> context)
        {
            var client = context.CreateRequestClient<Commands.CookDish>(_bus);
            await client.GetResponse<Events.DishCooked>(new Commands.CookDish
            {
                OrderId = context.Arguments.OrderId,
                OrderDetails = context.Arguments.OrderDetails
            });
            return context.Completed();
        }
    }
}