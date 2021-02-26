using CommunicationFoodDelivery.Activities;
using CommunicationFoodDelivery.Consumers;
using CommunicationFoodDelivery.Contracts;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicationFoodDelivery
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddMassTransit(x =>
                {
                    x.SetEndpointNameFormatter(DefaultEndpointNameFormatter.Instance);
                    
                    x.AddConsumer<CookDishConsumer>();
                    x.AddConsumer<OrderPlacedConsumer>();
                    x.AddConsumer<DeliverOrderConsumer>();

                    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                        .InMemoryRepository();

                    x.AddExecuteActivity<CookDishActivity, CookDishArgument>();
                    x.AddExecuteActivity<DeliverOrderActivity, DeliverOrderArgument>();

                    x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
                })
                .AddMassTransitHostedService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
