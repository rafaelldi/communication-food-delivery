using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using static CommunicationFoodDelivery.Contracts.Commands;
using static CommunicationFoodDelivery.Contracts.Events;

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
            var client = context.CreateRequestClient<DeliverOrder>(_bus);
            await client.GetResponse<OrderDelivered>(new DeliverOrder
            {
                OrderId = context.Arguments.OrderId,
                Address = context.Arguments.Address
            });
            return context.Completed();
        }
    }
}