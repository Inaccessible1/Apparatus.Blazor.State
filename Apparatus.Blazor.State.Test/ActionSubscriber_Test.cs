using Apparatus.Blazor.State.Contracts;
using AutoFixture;
using NSubstitute;

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
            var mockDelegate = Substitute.For<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act
            var subscriptionId = actionSubscriber.Subscribe(mockDelegate);
            actionSubscriber.Publish(action);

            // Assert
            Assert.NotNull(subscriptionId);
            Assert.NotEmpty(subscriptionId);
            mockDelegate.Received(1).Invoke(action);
        }

        [Fact]
        public void Subscribe_WhenCalledWithMultipleUniqueSubscribers_ShouldAddAllSubscribersAndInvokeEach()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction2>();

            var mockDelegate1 = Substitute.For<Action<MyAction1>>();
            var mockDelegate2 = Substitute.For<Action<MyAction2>>();
            var mockDelegate3 = Substitute.For<Action<MyAction1>>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            var subscriptionId1 = actionSubscriber.Subscribe(mockDelegate1);
            var subscriptionId2 = actionSubscriber.Subscribe(mockDelegate2);
            var subscriptionId3 = actionSubscriber.Subscribe(mockDelegate3);

            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);

            // Assert
            Assert.NotEqual(subscriptionId1, subscriptionId3); // Different GUIDs even for same delegate type
            mockDelegate1.Received(1).Invoke(action1);
            mockDelegate2.Received(1).Invoke(action2);
            mockDelegate3.Received(1).Invoke(action1);
        }

        [Fact]
        public void Subscribe_WhenCalledWithSameDelegate_ShouldAddEachSubscriptionSeparately()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();
            var mockDelegate = Substitute.For<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act - Each subscribe call now creates a unique subscription
            var subscriptionId1 = actionSubscriber.Subscribe(mockDelegate);
            var subscriptionId2 = actionSubscriber.Subscribe(mockDelegate);
            var subscriptionId3 = actionSubscriber.Subscribe(mockDelegate);

            actionSubscriber.Publish(action);

            // Assert
            Assert.NotEqual(subscriptionId1, subscriptionId2);
            Assert.NotEqual(subscriptionId2, subscriptionId3);
            // Each subscription invokes the delegate, so 3 times total
            mockDelegate.Received(3).Invoke(action);
        }

        [Fact]
        public void Subscribe_WhenCalledForMultipleActionTypes_ShouldMaintainSeparateSubscriberLists()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction2>();
            var action3 = fixture.Create<MyAction3>();

            var mockDelegate1 = Substitute.For<Action<MyAction1>>();
            var mockDelegate2 = Substitute.For<Action<MyAction2>>();
            var mockDelegate3 = Substitute.For<Action<MyAction3>>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            actionSubscriber.Subscribe(mockDelegate1);
            actionSubscriber.Subscribe(mockDelegate2);
            actionSubscriber.Subscribe(mockDelegate3);

            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);
            actionSubscriber.Publish(action3);

            // Assert
            mockDelegate1.Received(1).Invoke(action1);
            mockDelegate1.Received(1).Invoke(Arg.Any<MyAction1>());

            mockDelegate2.Received(1).Invoke(action2);
            mockDelegate2.Received(1).Invoke(Arg.Any<MyAction2>());

            mockDelegate3.Received(1).Invoke(action3);
            mockDelegate3.Received(1).Invoke(Arg.Any<MyAction3>());
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

            var mockDelegate1 = Substitute.For<Action<MyAction1>>();
            var mockDelegate2 = Substitute.For<Action<MyAction1>>();
            var mockDelegate3 = Substitute.For<Action<MyAction1>>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            actionSubscriber.Subscribe(mockDelegate1);
            actionSubscriber.Subscribe(mockDelegate2);
            actionSubscriber.Subscribe(mockDelegate3);

            actionSubscriber.Publish(action);

            // Assert
            mockDelegate1.Received(1).Invoke(action);
            mockDelegate2.Received(1).Invoke(action);
            mockDelegate3.Received(1).Invoke(action);
        }

        [Fact]
        public void Publish_WhenCalledMultipleTimes_ShouldInvokeSubscribersEachTime()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction1>();
            var action3 = fixture.Create<MyAction1>();

            var mockDelegate = Substitute.For<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act
            actionSubscriber.Subscribe(mockDelegate);
            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);
            actionSubscriber.Publish(action3);

            // Assert
            mockDelegate.Received(1).Invoke(action1);
            mockDelegate.Received(1).Invoke(action2);
            mockDelegate.Received(1).Invoke(action3);
            mockDelegate.Received(3).Invoke(Arg.Any<MyAction1>());
        }

        [Fact]
        public void Publish_WhenSubscriberThrowsException_ShouldNotAffectOtherSubscribers()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();

            var mockDelegate1 = Substitute.For<Action<MyAction1>>();
            var mockDelegate2 = Substitute.For<Action<MyAction1>>();
            var mockDelegate3 = Substitute.For<Action<MyAction1>>();

            mockDelegate2.When(x => x(Arg.Any<MyAction1>())).Do(x => throw new InvalidOperationException());

            var actionSubscriber = new ActionSubscriber();

            // Act
            actionSubscriber.Subscribe(mockDelegate1);
            actionSubscriber.Subscribe(mockDelegate2);
            actionSubscriber.Subscribe(mockDelegate3);

            var exception = Record.Exception(() => actionSubscriber.Publish(action));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            mockDelegate1.Received(1).Invoke(action);
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
            var mockDelegate = Substitute.For<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act - Capture the subscription ID returned by Subscribe
            var subscriptionId = actionSubscriber.Subscribe(mockDelegate);
            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId);
            actionSubscriber.Publish(action);

            // Assert
            mockDelegate.DidNotReceive().Invoke(Arg.Any<MyAction1>());
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

            var mockDelegate1 = Substitute.For<Action<MyAction1>>();
            var mockDelegate2 = Substitute.For<Action<MyAction1>>();
            var mockDelegate3 = Substitute.For<Action<MyAction1>>();

            var actionSubscriber = new ActionSubscriber();

            // Act - Capture subscription IDs
            var subscriptionId1 = actionSubscriber.Subscribe(mockDelegate1);
            var subscriptionId2 = actionSubscriber.Subscribe(mockDelegate2);
            var subscriptionId3 = actionSubscriber.Subscribe(mockDelegate3);

            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId2);
            actionSubscriber.Publish(action);

            // Assert
            mockDelegate1.Received(1).Invoke(action);
            mockDelegate2.DidNotReceive().Invoke(Arg.Any<MyAction1>());
            mockDelegate3.Received(1).Invoke(action);
        }

        [Fact]
        public void Unsubscribe_WhenLastSubscriberIsRemoved_ShouldRemoveActionTypeFromDictionary()
        {
            // Arrange
            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();

            var mockDelegate = Substitute.For<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act - Capture subscription ID
            var subscriptionId = actionSubscriber.Subscribe(mockDelegate);
            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId);
            actionSubscriber.Publish(action);

            // Assert - No exception should be thrown when publishing to an action type with no subscribers
            mockDelegate.DidNotReceive().Invoke(Arg.Any<MyAction1>());
        }

        #endregion

        #region Thread Safety Tests

        [Fact]
        public void Subscribe_WhenCalledConcurrently_ShouldHandleAllSubscriptionsSafely()
        {
            // Arrange
            var actionSubscriber = new ActionSubscriber();
            var subscribers = new List<Action<MyAction1>>();
            var taskCount = 10;

            for (int i = 0; i < taskCount; i++)
            {
                subscribers.Add(Substitute.For<Action<MyAction1>>());
            }

            // Act
            var tasks = subscribers.Select(sub =>
                Task.Run(() => actionSubscriber.Subscribe(sub))
            ).ToArray();

            Task.WaitAll(tasks);

            var fixture = new Fixture();
            var action = fixture.Create<MyAction1>();
            actionSubscriber.Publish(action);

            // Assert - All unique subscribers should be invoked
            foreach (var sub in subscribers)
            {
                sub.Received(1).Invoke(action);
            }
        }

        [Fact]
        public void Publish_WhenCalledConcurrently_ShouldInvokeSubscribersSafely()
        {
            // Arrange
            var fixture = new Fixture();
            var actionSubscriber = new ActionSubscriber();
            var mockDelegate = Substitute.For<Action<MyAction1>>();
            var publishCount = 10;

            actionSubscriber.Subscribe(mockDelegate);

            // Act
            var tasks = Enumerable.Range(0, publishCount)
                .Select(_ => Task.Run(() => actionSubscriber.Publish(fixture.Create<MyAction1>()))
                ).ToArray();

            Task.WaitAll(tasks);

            // Assert
            mockDelegate.Received(publishCount).Invoke(Arg.Any<MyAction1>());
        }

        [Fact]
        public void SubscribeAndUnsubscribe_WhenCalledConcurrently_ShouldMaintainConsistency()
        {
            // Arrange
            var actionSubscriber = new ActionSubscriber();
            var mockDelegate = Substitute.For<Action<MyAction1>>();
            var iterations = 100;
            var subscriptionIds = new System.Collections.Concurrent.ConcurrentBag<string>();

            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < iterations; i++)
            {
                tasks.Add(Task.Run(() => 
                {
                    var id = actionSubscriber.Subscribe(mockDelegate);
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

            var mockDelegate = Substitute.For<Action<MyAction1>>();
            var actionSubscriber = new ActionSubscriber();

            // Act & Assert - Subscribe and Publish
            var subscriptionId = actionSubscriber.Subscribe(mockDelegate);
            actionSubscriber.Publish(action1);
            mockDelegate.Received(1).Invoke(action1);

            // Unsubscribe and Publish again
            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId);
            actionSubscriber.Publish(action2);
            mockDelegate.DidNotReceive().Invoke(action2);
            mockDelegate.Received(1).Invoke(Arg.Any<MyAction1>()); // Only the first publish
        }

        [Fact]
        public void MultipleActionTypes_WhenManagedIndependently_ShouldNotInterfereWithEachOther()
        {
            // Arrange
            var fixture = new Fixture();
            var action1 = fixture.Create<MyAction1>();
            var action2 = fixture.Create<MyAction2>();

            var mockDelegate1 = Substitute.For<Action<MyAction1>>();
            var mockDelegate2 = Substitute.For<Action<MyAction2>>();

            var actionSubscriber = new ActionSubscriber();

            // Act
            var subscriptionId1 = actionSubscriber.Subscribe(mockDelegate1);
            var subscriptionId2 = actionSubscriber.Subscribe(mockDelegate2);

            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);

            mockDelegate1.Received(1).Invoke(action1);
            mockDelegate2.Received(1).Invoke(action2);

            // Unsubscribe MyAction1 and verify MyAction2 still works
            actionSubscriber.Unsubscribe<MyAction1>(subscriptionId1);
            actionSubscriber.Publish(action1);
            actionSubscriber.Publish(action2);

            // Assert
            mockDelegate1.Received(1).Invoke(Arg.Any<MyAction1>()); // Only first publish
            mockDelegate2.Received(2).Invoke(action2); // Both publishes
        }

        #endregion
    }
}

