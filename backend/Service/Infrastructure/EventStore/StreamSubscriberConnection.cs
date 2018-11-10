﻿using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Service
{
    public class StreamSubscriberConnection : ISubscriptionEventStoreConnection
    {
        private readonly string connectionString;
        private Lazy<Task<IEventStoreConnection>> connection;

        public StreamSubscriberConnection(string connectionString)
        {
            this.connectionString = connectionString;
            Reconnect();
        }

        public EventStoreAllCatchUpSubscription SubscribeToAllFrom(Position? lastCheckpoint, CatchUpSubscriptionSettings settings,
            Func<EventStoreCatchUpSubscription, ResolvedEvent, Task> eventAppeared, Action<EventStoreCatchUpSubscription> liveProcessingStarted = null,
            Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null, UserCredentials userCredentials = null)
        {
            return connection.Value.Result.SubscribeToAllFrom(lastCheckpoint, settings, eventAppeared, liveProcessingStarted, subscriptionDropped,
                userCredentials);
        }

        private void Reconnect()
        {
            connection = new Lazy<Task<IEventStoreConnection>>(InitializeConnection);
        }

        private async Task<IEventStoreConnection> InitializeConnection()
        {
            var conn = EventStoreConnection.Create(connectionString);
            conn.Closed += (sender, args) => Reconnect();
            await conn.ConnectAsync();
            return conn;
        }

        public void Dispose()
        {
            connection.Value.Result.Dispose();
        }
    }
}
