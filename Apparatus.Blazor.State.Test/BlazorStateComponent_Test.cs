using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Apparatus.Blazor.State.Test
{
    public class BlazorStateComponent_Test
    {
        public class MyState : IState { }

        public class MyBlazorComponent : BlazorStateComponent { }

        [Fact]
        public void GetState_Test()
        {
            //Setup
            var fixture = new Fixture();
            var state = fixture.Create<MyState>();

            var mockSubscriptionService = new Mock<ISubscriptionService>();

            using var ctx = new TestContext();
            ctx.Services.AddSingleton(state);
            ctx.Services.AddSingleton(mockSubscriptionService.Object);

            //Act
            var renderedComponent = ctx.RenderComponent<MyBlazorComponent>();
            var myState = renderedComponent.Instance.GetState<MyState>();

            //Assert
            mockSubscriptionService.Verify(mock => mock.Add(state.GetType(), renderedComponent.Instance), Times.Once);
        }

        [Fact]
        public void ReRender_Test()
        {
            //Setup
            var mockSubscriptionService = new Mock<ISubscriptionService>();

            using var ctx = new TestContext();
            ctx.Services.AddSingleton(mockSubscriptionService.Object);

            //Act
            var renderedComponent = ctx.RenderComponent<MyBlazorComponent>();
            renderedComponent.Instance.ReRender();

            //Assert
            Assert.Equal(2, renderedComponent.RenderCount);

        }

        [Fact]
        public void Dispose_Test()
        {
            //Setup
            var mockSubscriptionService = new Mock<ISubscriptionService>();

            using var ctx = new TestContext();
            ctx.Services.AddSingleton(mockSubscriptionService.Object);

            //Act
            var renderedComponent = ctx.RenderComponent<MyBlazorComponent>();
            renderedComponent.Instance.Dispose();

            // Assert
            mockSubscriptionService.Verify(mock => mock.Remove(It.IsAny<MyBlazorComponent>()), Times.Once);
        }


    }
}
