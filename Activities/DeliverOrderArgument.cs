using System;

namespace CommunicationFoodDelivery.Activities
{
    public record DeliverOrderArgument
    {
        public Guid OrderId { get; init; }
        public string Address { get; init; }
    }
}