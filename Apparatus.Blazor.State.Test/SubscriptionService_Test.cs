using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Apparatus.Blazor.State.Test
{
    public class SubscriptionService_Test
    {
        public class MyState : IState { };

        public class MyBlazorComponent : BlazorStateComponent { }

        public class MyBlazorComponent1 : BlazorStateComponent { }

        public class MyBlazorComponent2 : BlazorStateComponent { }

        [Fact]
        public void Add_Subscription_Test__ReRender_Added_Component()
        {
            //Setup
            var mockSubscriptionService = new Mock<ISubscriptionService>();

            using var ctx = new TestContext();
            ctx.Services.AddSingleton(mockSubscriptionService.Object);

            //Act
            var subscriptionService = new SubscriptionService();

            var renderedComponent = ctx.Render<MyBlazorComponent>(); //First render
            subscriptionService.Add(typeof(MyState), renderedComponent.Instance);
            subscriptionService.ReRenderSubscribers<MyState>(); //Second render

            //Assert
            Assert.Equal(2, renderedComponent.RenderCount);
        }

        [Fact]
        public void Add_Subscription_Test__Skip_Redundant_Component_For_Same_State()
        {
            //Setup
            var mockSubscriptionService = new Mock<ISubscriptionService>();

            using var ctx = new TestContext();
            ctx.Services.AddSingleton(mockSubscriptionService.Object);

            //Ac
            var subscriptionService = new SubscriptionService();

            var renderedComponent = ctx.Render<MyBlazorComponent>(); //First render
            subscriptionService.Add(typeof(MyState), renderedComponent.Instance);
            subscriptionService.Add(typeof(MyState), renderedComponent.Instance);

            subscriptionService.ReRenderSubscribers<MyState>(); //Second render

            //Assert
            Assert.Equal(2, renderedComponent.RenderCount);
        }

        [Fact]
        public void Add_Subscription_Test__ReRender_Multiple_Components()
        {
            //Setup
            var mockSubscriptionService = new Mock<ISubscriptionService>();

            using var ctx = new TestContext();
            ctx.Services.AddSingleton(mockSubscriptionService.Object);

            //Act
            var subscriptionService = new SubscriptionService();

            var renderedComponent1 = ctx.Render<MyBlazorComponent1>(); //First render
            var renderedComponent2 = ctx.Render<MyBlazorComponent2>(); //First render

            subscriptionService.Add(typeof(MyState), renderedComponent1.Instance);
            subscriptionService.Add(typeof(MyState), renderedComponent2.Instance);

            subscriptionService.ReRenderSubscribers<MyState>(); //Second render

            //Assert
            Assert.Equal(2, renderedComponent1.RenderCount);
            Assert.Equal(2, renderedComponent2.RenderCount);
        }

        [Fact]
        public void Remove_Subscription_Test()
        {
            //Setup
            var mockSubscriptionService = new Mock<ISubscriptionService>();

            using var ctx = new TestContext();
            ctx.Services.AddSingleton(mockSubscriptionService.Object);

            //Act
            var subscriptionService = new SubscriptionService();

            var renderedComponent = ctx.Render<MyBlazorComponent>(); //First render

            subscriptionService.Add(typeof(MyState), renderedComponent.Instance);
            subscriptionService.Remove(renderedComponent.Instance);
            subscriptionService.ReRenderSubscribers<MyState>();

            //Assert
            Assert.Equal(1, renderedComponent.RenderCount);
        }



    }
}
