using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunicationFoodDelivery.Activities;
using CommunicationFoodDelivery.Contracts;
using MassTransit;
using MassTransit.Courier;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationFoodDelivery.Controllers
{
    [ApiController]
    [Route("choreography/orders")]
    public class OrdersChoreographyController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly IEndpointNameFormatter _formatter;
        private static readonly Dictionary<Guid, (string OrderDetails, string Address)> _cash = new();

        public OrdersChoreographyController(IBus bus, IEndpointNameFormatter formatter)
        {
            _bus = bus;
            _formatter = formatter;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderDto dto)
        {
            var orderId = Guid.NewGuid();

            _cash.Add(orderId, (dto.OrderDetails, dto.Address));

            await _bus.Publish(new Events.OrderPlaced
            {
                OrderId = orderId,
                OrderDetails = dto.OrderDetails
            });

            return Ok();
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptOrder(Guid id)
        {
            if (!_cash.TryGetValue(id, out var order))
            {
                throw new ArgumentException("Can't find order details");
            }

            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            var cookDishActivity = _formatter.ExecuteActivity<CookDishActivity, CookDishArgument>();
            builder.AddActivity("CookDish", new Uri($"queue:{cookDishActivity}"), new CookDishArgument
            {
                OrderId = id,
                OrderDetails = order.OrderDetails
            });

            var deliverOrderActivity = _formatter.ExecuteActivity<DeliverOrderActivity, DeliverOrderArgument>();
            builder.AddActivity("DeliverOrder", new Uri($"queue:{deliverOrderActivity}"), new DeliverOrderArgument
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