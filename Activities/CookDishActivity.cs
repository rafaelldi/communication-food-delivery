using System.Threading.Tasks;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace CommunicationFoodDelivery.Activities
{
    public class CookDishActivity : IExecuteActivity<CookDishArgument>
    {
        private readonly ILogger<CookDishActivity> _logger;

        public CookDishActivity(ILogger<CookDishActivity> logger)
        {
            _logger = logger;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<CookDishArgument> context)
        {
            await Task.Delay(500);

            _logger.LogInformation("Dish for order with id = {id} was cooked", context.Arguments.OrderId);
            return context.Completed();
        }
    }
}