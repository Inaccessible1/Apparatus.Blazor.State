using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using Moq;

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

            var mockActionSubscriber = new Mock<IActionSubscriber>();
            var mockActionHandler = new Mock<IActionHandler<MyAction>>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider
                .Setup(mock => mock.GetService(typeof(IActionHandler<MyAction>)))
                .Returns(mockActionHandler.Object);

            //Act
            var dispatcher = new ActionDispatcher(mockServiceProvider.Object, mockActionSubscriber.Object);
            await dispatcher.Dispatch(action);

            //Assert
            mockActionHandler.Verify(mock => mock.Handle(action), Times.Once);
            mockActionSubscriber.Verify(mock => mock.Publish(action), Times.Once);
        }


        [Fact]
        public async Task Dispatch_Dynamic_Action_Associated_With_Handler()
        {
            //Setup
            var fixture = new Fixture();
            var action = fixture.Create<MyAction>();

            var mockActionSubscriber = new Mock<IActionSubscriber>();
            var mockActionHandler = new Mock<IActionHandler<MyAction>>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider
                .Setup(mock => mock.GetService(typeof(IActionHandler<MyAction>)))
                .Returns(mockActionHandler.Object);

            //Act
            var dispatcher = new ActionDispatcher(mockServiceProvider.Object, mockActionSubscriber.Object);
            IAction iAction = action;
            await dispatcher.Dispatch(iAction);


            //Assert
            mockActionSubscriber.Verify(mock => mock.Publish(iAction), Times.Once);
            mockActionHandler.Verify(mock => mock.Handle(It.IsAny<MyAction>()), Times.Once);
        }
    }
}