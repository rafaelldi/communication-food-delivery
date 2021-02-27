using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunicationFoodDelivery.Activities;
using MassTransit;
using MassTransit.Courier;
using Microsoft.AspNetCore.Mvc;
using static CommunicationFoodDelivery.Contracts.Events;

namespace CommunicationFoodDelivery.Controllers
{
    [ApiController]
    [Route("choreography/orders")]
    public class OrdersChoreographyController : ControllerBase
    {
        private readonly IBus _bus;
        private static readonly Dictionary<Guid, (string OrderDetails, string Address)> Cash = new();

        public OrdersChoreographyController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderDto dto)
        {
            var orderId = Guid.NewGuid();

            Cash.Add(orderId, (dto.OrderDetails, dto.Address));

            await _bus.Publish(new OrderPlaced
            {
                OrderId = orderId,
                OrderDetails = dto.OrderDetails
            });

            return Ok();
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptOrder(Guid id)
        {
            if (!Cash.TryGetValue(id, out var order))
            {
                throw new ArgumentException("Can't find order details");
            }

            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            builder.AddActivity("CookDish", new Uri("queue:CookDish_execute"), new CookDishArgument
            {
                OrderId = id,
                OrderDetails = order.OrderDetails
            });

            builder.AddActivity("DeliverOrder", new Uri("queue:DeliverOrder_execute"), new DeliverOrderArgument
            {
                OrderId = id,
                Address = order.Address
            });

            var routingSlip = builder.Build();

            await _bus.Execute(routingSlip);
            return Ok();
        }
    }
}