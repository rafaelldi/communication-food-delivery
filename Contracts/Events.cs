using System;

namespace CommunicationFoodDelivery.Contracts
{
    public static class Events
    {
        public record OrderPlaced
        {
            public Guid OrderId { get; init; }
            public string OrderDetails { get; init; }
        }

        public record DishCooked
        {
            public Guid OrderId { get; init; }
        }

        public record OrderDelivered
        {
            public Guid OrderId { get; init; }
        }
    }
}