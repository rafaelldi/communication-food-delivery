using System;

namespace CommunicationFoodDelivery.Activities
{
    public record CookDishArgument
    {
        public Guid OrderId { get; init; }
        public string OrderDetails { get; init; }
    }
}