using System;

namespace CommunicationFoodDelivery.Contracts
{
    public static class Commands
    {
        public record PlaceOrder
        {
            public Guid OrderId { get; init; }
            public string OrderDetails { get; init; }
            public string Address { get; init; }
        }

        public record AcceptOrder
        {
            public Guid OrderId { get; init; }
        }

        public record CookDish
        {
            public Guid OrderId { get; init; }
            public string OrderDetails { get; init; }
        }

        public record DeliverOrder
        {
            public Guid OrderId { get; init; }
            public string Address { get; init; }
        }
    }
}