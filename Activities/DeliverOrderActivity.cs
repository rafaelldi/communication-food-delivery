using System.Threading.Tasks;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace CommunicationFoodDelivery.Activities
{
    public class DeliverOrderActivity : IExecuteActivity<DeliverOrderArgument>
    {
        private readonly ILogger<DeliverOrderActivity> _logger;

        public DeliverOrderActivity(ILogger<DeliverOrderActivity> logger)
        {
            _logger = logger;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<DeliverOrderArgument> context)
        {
            await Task.Delay(500);

            _logger.LogInformation("Order with id = {id} was delivered", context.Arguments.OrderId);
            return context.Completed();
        }
    }
}