using System.Threading.Tasks;
using CommunicationFoodDelivery.Contracts;
using MassTransit;
using MassTransit.Courier;

namespace CommunicationFoodDelivery.Activities
{
    public class DeliverOrderActivity : IExecuteActivity<DeliverOrderArgument>
    {
        private readonly IBus _bus;
        
        public DeliverOrderActivity(IBus bus)
        {
            _bus = bus;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<DeliverOrderArgument> context)
        {
            var client = context.CreateRequestClient<Commands.DeliverOrder>(_bus);
            await client.GetResponse<Events.OrderDelivered>(new Commands.DeliverOrder
            {
                OrderId = context.Arguments.OrderId,
                Address = context.Arguments.Address
            });
            return context.Completed();
        }
    }
}