using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using Moq;

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

            var mockSubscriptionService = new Mock<ISubscriptionService>();

            //Act
            var store = new Store<MyState>(oldState, mockSubscriptionService.Object) { };
            await store.SetState(newState);

            //Assert
            Assert.Equal(oldState, store.State);
            mockSubscriptionService.Verify(mock => mock.ReRenderSubscribers<MyState>(), Times.Once);
        }

        [Fact]
        public async Task Refresh_Test()
        {
            //Setup
            var fixture = new Fixture();
            var oldState = fixture.Create<MyState>();
            var newState = fixture.Create<MyState>();

            var mockSubscriptionService = new Mock<ISubscriptionService>();

            //Act
            var store = new Store<MyState>(oldState, mockSubscriptionService.Object) { };
            await store.Refresh();

            //Assert
            Assert.Equal(oldState, store.State);
            mockSubscriptionService.Verify(mock => mock.ReRenderSubscribers<MyState>(), Times.Once);
        }
    }
}
