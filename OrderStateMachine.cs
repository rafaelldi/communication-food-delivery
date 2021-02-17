using System;
using Automatonymous;
using Automatonymous.Binders;
using MassTransit;
using static CommunicationFoodDelivery.Contracts.Commands;
using static CommunicationFoodDelivery.Contracts.Events;

namespace CommunicationFoodDelivery
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }

        public Guid OrderId { get; set; }
        public string OrderDetails { get; set; }
        public string Address { get; set; }
        public DateTime Placed { get; set; }
        public DateTime Accepted { get; set; }
        public DateTime Cooked { get; set; }
        public DateTime Delivered { get; set; }
    }

    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState, Placed, Accepted, Cooked);

            Event(() => PlaceOrder, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => AcceptOrder, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => DishCooked, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderDelivered, x => x.CorrelateById(m => m.Message.OrderId));

            Initially(
                When(PlaceOrder)
                    .SetOrderDetails()
                    .TransitionTo(Placed)
                    .PublishOrderPlaced());

            During(Placed,
                When(AcceptOrder)
                    .SetAcceptedTime()
                    .TransitionTo(Accepted)
                    .PublishCookDish());

            During(Accepted,
                When(DishCooked)
                    .SetCookedTime()
                    .TransitionTo(Cooked)
                    .PublishDeliverOrder());

            During(Cooked,
                When(OrderDelivered)
                    .SetDeliveredTime()
                    .Finalize());
        }

        public Event<PlaceOrder> PlaceOrder { get; private set; }
        public Event<AcceptOrder> AcceptOrder { get; private set; }
        public Event<DishCooked> DishCooked { get; private set; }
        public Event<OrderDelivered> OrderDelivered { get; private set; }

        public State Placed { get; private set; }
        public State Accepted { get; private set; }
        public State Cooked { get; private set; }
    }

    public static class OrderStateMachineExtensions
    {
        public static EventActivityBinder<OrderState, PlaceOrder> SetOrderDetails(
            this EventActivityBinder<OrderState, PlaceOrder> binder)
        {
            return binder.Then(x =>
            {
                x.Instance.OrderId = x.Data.OrderId;
                x.Instance.OrderDetails = x.Data.OrderDetails;
                x.Instance.Address = x.Data.Address;
                x.Instance.Placed = DateTime.UtcNow;
            });
        }

        public static EventActivityBinder<OrderState, PlaceOrder> PublishOrderPlaced(
            this EventActivityBinder<OrderState, PlaceOrder> binder)
        {
            return binder.PublishAsync(context => context.Init<OrderPlaced>(new OrderPlaced
            {
                OrderId = context.Data.OrderId,
                OrderDetails = context.Data.OrderDetails
            }));
        }

        public static EventActivityBinder<OrderState, AcceptOrder> SetAcceptedTime(
            this EventActivityBinder<OrderState, AcceptOrder> binder)
        {
            return binder.Then(x => { x.Instance.Accepted = DateTime.UtcNow; });
        }

        public static EventActivityBinder<OrderState, AcceptOrder> PublishCookDish(
            this EventActivityBinder<OrderState, AcceptOrder> binder)
        {
            return binder.PublishAsync(context => context.Init<CookDish>(new CookDish
            {
                OrderId = context.Data.OrderId,
                OrderDetails = context.Instance.OrderDetails
            }));
        }

        public static EventActivityBinder<OrderState, DishCooked> SetCookedTime(
            this EventActivityBinder<OrderState, DishCooked> binder)
        {
            return binder.Then(x => { x.Instance.Cooked = DateTime.UtcNow; });
        }

        public static EventActivityBinder<OrderState, DishCooked> PublishDeliverOrder(
            this EventActivityBinder<OrderState, DishCooked> binder)
        {
            return binder.PublishAsync(context => context.Init<DeliverOrder>(new DeliverOrder
            {
                OrderId = context.Data.OrderId,
                Address = context.Instance.Address
            }));
        }

        public static EventActivityBinder<OrderState, OrderDelivered> SetDeliveredTime(
            this EventActivityBinder<OrderState, OrderDelivered> binder)
        {
            return binder.Then(x => { x.Instance.Delivered = DateTime.UtcNow; });
        }
    }
}