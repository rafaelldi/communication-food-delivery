using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using static CommunicationFoodDelivery.Contracts.Commands;
using static CommunicationFoodDelivery.Contracts.Events;

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
            var client = context.CreateRequestClient<CookDish>(_bus);
            await client.GetResponse<DishCooked>(new CookDish
            {
                OrderId = context.Arguments.OrderId,
                OrderDetails = context.Arguments.OrderDetails
            });
            return context.Completed();
        }
    }
}