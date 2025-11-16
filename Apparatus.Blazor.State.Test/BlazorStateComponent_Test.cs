using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Apparatus.Blazor.State.Test
{
    public class BlazorStateComponent_Test
    {
        public class MyState : IState { }

        public class MyBlazorComponent : BlazorStateComponent { }

        [Fact]
        public void GetState__When_State_Is_Resolved_Test()
        {
            //Setup
            var fixture = new Fixture();
            var state = fixture.Create<MyState>();

            var mockSubscriptionService = Substitute.For<ISubscriptionService>();

            using var ctx = new BunitContext();
            ctx.Services.AddSingleton(state);
            ctx.Services.AddSingleton(mockSubscriptionService);

            //Act
            var renderedComponent = ctx.Render<MyBlazorComponent>();
            var myState = renderedComponent.Instance.GetState<MyState>();

            //Assert
            mockSubscriptionService.Received(1).Add(state.GetType(), renderedComponent.Instance);
        }

        [Fact]
        public void GetState__When_State_Cannot_Be_Resolved_Test()
        {
            //Setup
            var fixture = new Fixture();
            var state = fixture.Create<MyState>();

            var mockSubscriptionService = Substitute.For<ISubscriptionService>();

            using var ctx = new BunitContext();
            //ctx.Services.AddSingleton(state);
            ctx.Services.AddSingleton(mockSubscriptionService);

            //Act
            var renderedComponent = ctx.Render<MyBlazorComponent>();
            var myState = renderedComponent.Instance.GetState<MyState>();

            //Assert
            Assert.NotNull(myState);
            mockSubscriptionService.DidNotReceive().Add(state.GetType(), renderedComponent.Instance);
        }

        [Fact]
        public void ReRender_Test()
        {
            //Setup
            var mockSubscriptionService = Substitute.For<ISubscriptionService>();

            using var ctx = new BunitContext();
            ctx.Services.AddSingleton(mockSubscriptionService);

            //Act
            var renderedComponent = ctx.Render<MyBlazorComponent>();
            renderedComponent.Instance.ReRender();

            //Assert
            Assert.Equal(2, renderedComponent.RenderCount);

        }

        [Fact]
        public void Dispose_Test()
        {
            //Setup
            var mockSubscriptionService = Substitute.For<ISubscriptionService>();

            using var ctx = new BunitContext();
            ctx.Services.AddSingleton(mockSubscriptionService);

            //Act
            var renderedComponent = ctx.Render<MyBlazorComponent>();
            renderedComponent.Instance.Dispose();

            // Assert
            mockSubscriptionService.Received(1).Remove(Arg.Any<MyBlazorComponent>());
        }


    }
}
