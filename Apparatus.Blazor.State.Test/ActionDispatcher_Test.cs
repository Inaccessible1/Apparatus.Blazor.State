using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using NSubstitute;

namespace Apparatus.Blazor.State.Test
{
    public class ActionDispatcher_Test
    {
        public class MyAction : IAction { }

        [Fact]
        public async Task Dispatch_Action_Associated_With_Handler()
        {
            //Setup
            var fixture = new Fixture();
            var action = fixture.Create<MyAction>();

            var mockActionSubscriber = Substitute.For<IActionSubscriber>();
            var mockActionHandler = Substitute.For<IActionHandler<MyAction>>();
            var mockServiceProvider = Substitute.For<IServiceProvider>();

            mockServiceProvider
                .GetService(typeof(IActionHandler<MyAction>))
                .Returns(mockActionHandler);

            //Act
            var dispatcher = new ActionDispatcher(mockServiceProvider, mockActionSubscriber);
            await dispatcher.Dispatch(action);

            //Assert
            await mockActionHandler.Received(1).Handle(action);
            mockActionSubscriber.Received(1).Publish(action);
        }


        [Fact]
        public async Task Dispatch_Dynamic_Action_Associated_With_Handler()
        {
            //Setup
            var fixture = new Fixture();
            var action = fixture.Create<MyAction>();

            var mockActionSubscriber = Substitute.For<IActionSubscriber>();
            var mockActionHandler = Substitute.For<IActionHandler<MyAction>>();
            var mockServiceProvider = Substitute.For<IServiceProvider>();

            mockServiceProvider
                .GetService(typeof(IActionHandler<MyAction>))
                .Returns(mockActionHandler);

            //Act
            var dispatcher = new ActionDispatcher(mockServiceProvider, mockActionSubscriber);
            IAction iAction = action;
            await dispatcher.Dispatch(iAction);


            //Assert
            mockActionSubscriber.Received(1).Publish(iAction);
            await mockActionHandler.Received(1).Handle(Arg.Any<MyAction>());
        }
    }
}