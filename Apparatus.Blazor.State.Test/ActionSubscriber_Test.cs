using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using Moq;

namespace Apparatus.Blazor.State.Test
{
    public class ActionSubscriber_Test
    {
        public class MyAction1 : IAction { }
        public class MyAction2 : IAction { }

        [Fact]
        public void Subscribe_And_Publish_Action_To_All_Unique_Subscribers()
        {
            //Setup
            var fixture = new Fixture();

            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction2>();

            var mockDelegate1 = new Mock<Action<MyAction1>>();
            var mockDelegate2 = new Mock<Action<MyAction2>>();
            var mockDelegate3 = new Mock<Action<MyAction1>>();

            //Act
            var actionSubscriber = new ActionSubscriber();

            actionSubscriber.Subscribe(mockDelegate1.Object);
            actionSubscriber.Subscribe(mockDelegate1.Object);
            actionSubscriber.Subscribe(mockDelegate2.Object);
            actionSubscriber.Subscribe(mockDelegate2.Object);
            actionSubscriber.Subscribe(mockDelegate3.Object);
            actionSubscriber.Subscribe(mockDelegate3.Object);

            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);

            //Assert
            mockDelegate1.Verify(mock => mock(action1), Times.Once);
            mockDelegate2.Verify(mock => mock(action2), Times.Once);
            mockDelegate3.Verify(mock => mock(action1), Times.Once);
        }

    }
}

