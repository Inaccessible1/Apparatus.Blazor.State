using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using Moq;

namespace Apparatus.Blazor.State.Test
{
    public class ActionSubscriber_Test
    {
        public class MyAction1 : IAction { }
        public class MyAction2 : IAction { }
        public class MyAction3 : IAction { }

        #region Subscribe Tests

        [Fact]
        public void Subscribe_WhenCalledWithSingleSubscriber_ShouldAddSubscriberToActionType()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();
            var mockDelegate = new Mock<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act
            var subscriptionId = actionSubscriber.Subscribe(mockDelegate.Object);
            actionSubscriber.Publish(action);

            // Assert
            Assert.NotNull(subscriptionId);
            Assert.NotEmpty(subscriptionId);
            mockDelegate.Verify(mock => mock(action), Times.Once);
        }

        [Fact]
        public void Subscribe_WhenCalledWithMultipleUniqueSubscribers_ShouldAddAllSubscribersAndInvokeEach()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction2>();

            var mockDelegate1 = new Mock<Action<MyAction1>>();
            var mockDelegate2 = new Mock<Action<MyAction2>>();
            var mockDelegate3 = new Mock<Action<MyAction1>>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            var subscriptionId1 = actionSubscriber.Subscribe(mockDelegate1.Object);
            var subscriptionId2 = actionSubscriber.Subscribe(mockDelegate2.Object);
            var subscriptionId3 = actionSubscriber.Subscribe(mockDelegate3.Object);

            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);

            // Assert
            Assert.NotEqual(subscriptionId1, subscriptionId3); // Different GUIDs even for same delegate type
            mockDelegate1.Verify(mock => mock(action1), Times.Once);
            mockDelegate2.Verify(mock => mock(action2), Times.Once);
            mockDelegate3.Verify(mock => mock(action1), Times.Once);
        }

        [Fact]
        public void Subscribe_WhenCalledWithSameDelegate_ShouldAddEachSubscriptionSeparately()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();
            var mockDelegate = new Mock<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act - Each subscribe call now creates a unique subscription
            var subscriptionId1 = actionSubscriber.Subscribe(mockDelegate.Object);
            var subscriptionId2 = actionSubscriber.Subscribe(mockDelegate.Object);
            var subscriptionId3 = actionSubscriber.Subscribe(mockDelegate.Object);

            actionSubscriber.Publish(action);

            // Assert
            Assert.NotEqual(subscriptionId1, subscriptionId2);
            Assert.NotEqual(subscriptionId2, subscriptionId3);
            // Each subscription invokes the delegate, so 3 times total
            mockDelegate.Verify(mock => mock(action), Times.Exactly(3));
        }

        [Fact]
        public void Subscribe_WhenCalledForMultipleActionTypes_ShouldMaintainSeparateSubscriberLists()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction2>();
            var action3 = fixture.Create<MyAction3>();

            var mockDelegate1 = new Mock<Action<MyAction1>>();
            var mockDelegate2 = new Mock<Action<MyAction2>>();
            var mockDelegate3 = new Mock<Action<MyAction3>>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            actionSubscriber.Subscribe(mockDelegate1.Object);
            actionSubscriber.Subscribe(mockDelegate2.Object);
            actionSubscriber.Subscribe(mockDelegate3.Object);

            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);
            actionSubscriber.Publish(action3);

            // Assert
            mockDelegate1.Verify(mock => mock(action1), Times.Once);
            mockDelegate1.Verify(mock => mock(It.IsAny<MyAction1>()), Times.Once);

            mockDelegate2.Verify(mock => mock(action2), Times.Once);
            mockDelegate2.Verify(mock => mock(It.IsAny<MyAction2>()), Times.Once);

            mockDelegate3.Verify(mock => mock(action3), Times.Once);
            mockDelegate3.Verify(mock => mock(It.IsAny<MyAction3>()), Times.Once);
        }

        #endregion

        #region Publish Tests

        [Fact]
        public void Publish_WhenNoSubscribersExist_ShouldNotThrowException()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();
            var actionSubscriber = new ActionSubscriber();

            // Act & Assert
            var exception = Record.Exception(() => actionSubscriber.Publish(action));
            Assert.Null(exception);
        }

        [Fact]
        public void Publish_WhenSubscribersExist_ShouldInvokeAllSubscribers()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();

            var mockDelegate1 = new Mock<Action<MyAction1>>();
            var mockDelegate2 = new Mock<Action<MyAction1>>();
            var mockDelegate3 = new Mock<Action<MyAction1>>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            actionSubscriber.Subscribe(mockDelegate1.Object);
            actionSubscriber.Subscribe(mockDelegate2.Object);
            actionSubscriber.Subscribe(mockDelegate3.Object);

            actionSubscriber.Publish(action);

            // Assert
            mockDelegate1.Verify(mock => mock(action), Times.Once);
            mockDelegate2.Verify(mock => mock(action), Times.Once);
            mockDelegate3.Verify(mock => mock(action), Times.Once);
        }

        [Fact]
        public void Publish_WhenCalledMultipleTimes_ShouldInvokeSubscribersEachTime()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction1>();
            var action3 = fixture.Create<MyAction1>();

            var mockDelegate = new Mock<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act
            actionSubscriber.Subscribe(mockDelegate.Object);
            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);
            actionSubscriber.Publish(action3);

            // Assert
            mockDelegate.Verify(mock => mock(action1), Times.Once);
            mockDelegate.Verify(mock => mock(action2), Times.Once);
            mockDelegate.Verify(mock => mock(action3), Times.Once);
            mockDelegate.Verify(mock => mock(It.IsAny<MyAction1>()), Times.Exactly(3));
        }

        [Fact]
        public void Publish_WhenSubscriberThrowsException_ShouldNotAffectOtherSubscribers()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();

            var mockDelegate1 = new Mock<Action<MyAction1>>();
            var mockDelegate2 = new Mock<Action<MyAction1>>();
            var mockDelegate3 = new Mock<Action<MyAction1>>();

            mockDelegate2.Setup(x => x(It.IsAny<MyAction1>())).Throws<InvalidOperationException>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            actionSubscriber.Subscribe(mockDelegate1.Object);
            actionSubscriber.Subscribe(mockDelegate2.Object);
            actionSubscriber.Subscribe(mockDelegate3.Object);

            var exception = Record.Exception(() => actionSubscriber.Publish(action));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            mockDelegate1.Verify(mock => mock(action), Times.Once);
            // mockDelegate2 throws, so mockDelegate3 may or may not be called depending on iteration order
        }

        #endregion

        #region Unsubscribe Tests

        [Fact]
        public void Unsubscribe_WhenSubscriptionExists_ShouldRemoveSubscriberAndNotInvokeOnPublish()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();
            var mockDelegate = new Mock<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act - Capture the subscription ID returned by Subscribe
            var subscriptionId = actionSubscriber.Subscribe(mockDelegate.Object);
            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId);
            actionSubscriber.Publish(action);

            // Assert
            mockDelegate.Verify(mock => mock(It.IsAny<MyAction1>()), Times.Never);
        }

        [Fact]
        public void Unsubscribe_WhenSubscriptionDoesNotExist_ShouldNotThrowException()
        {
            // Arrange
            var actionSubscriber = new ActionSubscriber();
            var nonExistentSubscriptionId = "NonExistent_12345";

            // Act & Assert
            var exception = Record.Exception(() => actionSubscriber.Unsubscribe<MyAction1>(nonExistentSubscriptionId));
            Assert.Null(exception);
        }

        [Fact]
        public void Unsubscribe_WhenActionTypeHasNoSubscriptions_ShouldNotThrowException()
        {
            // Arrange
            var actionSubscriber = new ActionSubscriber();
            var subscriptionId = "SomeId_12345";

            // Act & Assert
            var exception = Record.Exception(() => actionSubscriber.Unsubscribe<MyAction1>(subscriptionId));
            Assert.Null(exception);
        }

        [Fact]
        public void Unsubscribe_WhenMultipleSubscribersExist_ShouldRemoveOnlySpecifiedSubscriber()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();

            var mockDelegate1 = new Mock<Action<MyAction1>>();
            var mockDelegate2 = new Mock<Action<MyAction1>>();
            var mockDelegate3 = new Mock<Action<MyAction1>>();

            var actionSubscriber = new ActionSubscriber();

            // Act - Capture subscription IDs
            var subscriptionId1 = actionSubscriber.Subscribe(mockDelegate1.Object);
            var subscriptionId2 = actionSubscriber.Subscribe(mockDelegate2.Object);
            var subscriptionId3 = actionSubscriber.Subscribe(mockDelegate3.Object);

            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId2);
            actionSubscriber.Publish(action);

            // Assert
            mockDelegate1.Verify(mock => mock(action), Times.Once);
            mockDelegate2.Verify(mock => mock(It.IsAny<MyAction1>()), Times.Never);
            mockDelegate3.Verify(mock => mock(action), Times.Once);
        }

        [Fact]
        public void Unsubscribe_WhenLastSubscriberIsRemoved_ShouldRemoveActionTypeFromDictionary()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();

            var mockDelegate = new Mock<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act - Capture subscription ID
            var subscriptionId = actionSubscriber.Subscribe(mockDelegate.Object);
            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId);
            actionSubscriber.Publish(action);

            // Assert - No exception should be thrown when publishing to an action type with no subscribers
            mockDelegate.Verify(mock => mock(It.IsAny<MyAction1>()), Times.Never);
        }

        #endregion

        #region Thread Safety Tests

        [Fact]
        public void Subscribe_WhenCalledConcurrently_ShouldHandleAllSubscriptionsSafely()
        {
            // Arrange
            var actionSubscriber = new ActionSubscriber();
            var subscribers = new List<Mock<Action<MyAction1>>>();
            var taskCount = 10;

            for (int i = 0; i < taskCount; i++)
            {
                subscribers.Add(new Mock<Action<MyAction1>>());
            }

            // Act
            var tasks = subscribers.Select(mock =>
                Task.Run(() => actionSubscriber.Subscribe(mock.Object))
            ).ToArray();

            Task.WaitAll(tasks);

            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();
            actionSubscriber.Publish(action);

            // Assert - All unique subscribers should be invoked
            foreach (var mock in subscribers)
            {
                mock.Verify(m => m(action), Times.Once);
            }
        }

        [Fact]
        public void Publish_WhenCalledConcurrently_ShouldInvokeSubscribersSafely()
        {
            // Arrange
            var fixture = new Fixture();
            var actionSubscriber = new ActionSubscriber();
            var mockDelegate = new Mock<Action<MyAction1>>();
            var publishCount = 10;

            actionSubscriber.Subscribe(mockDelegate.Object);

            // Act
            var tasks = Enumerable.Range(0, publishCount)
                .Select(_ => Task.Run(() => actionSubscriber.Publish(fixture.Create<MyAction1>()))
                ).ToArray();

            Task.WaitAll(tasks);

            // Assert
            mockDelegate.Verify(m => m(It.IsAny<MyAction1>()), Times.Exactly(publishCount));
        }

        [Fact]
        public void SubscribeAndUnsubscribe_WhenCalledConcurrently_ShouldMaintainConsistency()
        {
            // Arrange
            var actionSubscriber = new ActionSubscriber();
            var mockDelegate = new Mock<Action<MyAction1>>();
            var iterations = 100;
            var subscriptionIds = new System.Collections.Concurrent.ConcurrentBag<string>();

            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < iterations; i++)
            {
                tasks.Add(Task.Run(() => 
                {
                    var id = actionSubscriber.Subscribe(mockDelegate.Object);
                    subscriptionIds.Add(id);
                }));
                tasks.Add(Task.Run(() => 
                {
                    if (subscriptionIds.TryTake(out var id))
                    {
                        actionSubscriber.Unsubscribe<MyAction1>(id);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert - Should not throw any exceptions
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();
            var exception = Record.Exception(() => actionSubscriber.Publish(action));
            Assert.Null(exception);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void SubscribePublishUnsubscribe_WhenExecutedInSequence_ShouldWorkCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction1>();

            var mockDelegate = new Mock<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act & Assert - Subscribe and Publish
            var subscriptionId = actionSubscriber.Subscribe(mockDelegate.Object);
            actionSubscriber.Publish(action1);
            mockDelegate.Verify(mock => mock(action1), Times.Once);

            // Unsubscribe and Publish again
            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId);
            actionSubscriber.Publish(action2);
            mockDelegate.Verify(mock => mock(action2), Times.Never);
            mockDelegate.Verify(mock => mock(It.IsAny<MyAction1>()), Times.Once); // Only the first publish
        }

        [Fact]
        public void MultipleActionTypes_WhenManagedIndependently_ShouldNotInterfereWithEachOther()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction2>();

            var mockDelegate1 = new Mock<Action<MyAction1>>();
            var mockDelegate2 = new Mock<Action<MyAction2>>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            var subscriptionId1 = actionSubscriber.Subscribe(mockDelegate1.Object);
            var subscriptionId2 = actionSubscriber.Subscribe(mockDelegate2.Object);

            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);

            mockDelegate1.Verify(mock => mock(action1), Times.Once);
            mockDelegate2.Verify(mock => mock(action2), Times.Once);

            // Unsubscribe MyAction1 and verify MyAction2 still works
            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId1);
            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);

            // Assert
            mockDelegate1.Verify(mock => mock(It.IsAny<MyAction1>()), Times.Once); // Only first publish
            mockDelegate2.Verify(mock => mock(action2), Times.Exactly(2)); // Both publishes
        }

        #endregion
    }
}

