using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using static CommunicationFoodDelivery.Contracts.Commands;

namespace CommunicationFoodDelivery.Controllers
{
    [ApiController]
    [Route("orchestration/orders")]
    public class OrdersOrchestrationController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersOrchestrationController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderDto dto)
        {
            await _publishEndpoint.Publish(new PlaceOrder
            {
                OrderId = Guid.NewGuid(),
                OrderDetails = dto.OrderDetails,
                Address = dto.Address
            });
            return Ok();
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptOrder(Guid id)
        {
            await _publishEndpoint.Publish(new AcceptOrder
            {
                OrderId = id
            });
            return Ok();
        }
    }
}