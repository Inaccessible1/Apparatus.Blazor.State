using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using NSubstitute;

namespace Apparatus.Blazor.State.Test
{
    public class Store_Test
    {
        public class MyState : IState { }

        [Fact]
        public async Task SetState_Test()
        {
            //Setup
            var fixture = new Fixture();
            var oldState = fixture.Create<MyState>();
            var newState = fixture.Create<MyState>();

            var mockSubscriptionService = Substitute.For<ISubscriptionService>();

            //Act
            var store = new Store<MyState>(oldState, mockSubscriptionService) { };
            await store.SetState(newState);

            //Assert
            Assert.Equal(newState, store.State);
            mockSubscriptionService.Received(1).ReRenderSubscribers<MyState>();
        }

        [Fact]
        public async Task Refresh_Test()
        {
            //Setup
            var fixture = new Fixture();
            var oldState = fixture.Create<MyState>();
            var newState = fixture.Create<MyState>();

            var mockSubscriptionService = Substitute.For<ISubscriptionService>();

            //Act
            var store = new Store<MyState>(oldState, mockSubscriptionService) { };
            await store.Refresh();

            //Assert
            Assert.Equal(oldState, store.State);
            mockSubscriptionService.Received(1).ReRenderSubscribers<MyState>();
        }
    }
}
