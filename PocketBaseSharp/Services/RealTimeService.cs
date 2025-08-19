using PocketBaseSharp.Models;
using PocketBaseSharp.Services.Base;
using PocketBaseSharp.Sse;

namespace PocketBaseSharp.Services
{
    /// <summary>
    /// Service for managing real-time connections and subscriptions using Server-Sent Events (SSE).
    /// Provides functionality to subscribe to collection changes and receive real-time updates.
    /// </summary>
    public class RealTimeService : BaseService
    {
        protected override string BasePath(string? path = null) => "/api/realtime";

        private readonly PocketBase _client;

        private SseClient? _sseClient = null;
        private SseClient SseClient => _sseClient ??= new SseClient(_client, RealTimeCallBackAsync);

        private readonly Dictionary<string, List<Func<SseMessage, Task>>> _subscriptions = new();

        /// <summary>
        /// Initializes a new instance of the RealTimeService class.
        /// </summary>
        /// <param name="client">The PocketBase client instance</param>
        public RealTimeService(PocketBase client)
        {
            this._client = client;
        }

        private async Task RealTimeCallBackAsync(SseMessage message)
        {
            var messageEvent = message.Event ?? "";
            if (_subscriptions.ContainsKey(messageEvent))
                foreach (var callBack in _subscriptions[messageEvent])
                    await callBack(message);
        }

        /// <summary>
        /// Subscribes to real-time events for a specific subscription topic.
        /// </summary>
        /// <param name="subscription">The subscription topic (e.g., collection name)</param>
        /// <param name="callback">The callback function to execute when messages are received</param>
        /// <returns>A task representing the asynchronous subscription operation</returns>
        public async Task SubscribeAsync(string subscription, Func<SseMessage, Task> callback)
        {
            if (!_subscriptions.ContainsKey(subscription))
            {
                // New subscription
                _subscriptions.Add(subscription, new List<Func<SseMessage, Task>> { callback });
                await SubmitSubscriptionsAsync();
            }
            else
            {
                var subcriptionCallbacks = _subscriptions[subscription];
                if (!subcriptionCallbacks.Contains(callback))
                    subcriptionCallbacks.Add(callback);
            }
        }

        /// <summary>
        /// Unsubscribes from real-time events for a specific topic or all topics.
        /// </summary>
        /// <param name="topic">The topic to unsubscribe from. If null or empty, unsubscribes from all topics</param>
        /// <returns>A task representing the asynchronous unsubscription operation</returns>
        public Task UnsubscribeAsync(string? topic = null)
        {
            if (string.IsNullOrEmpty(topic))
                _subscriptions.Clear();
            else if (_subscriptions.ContainsKey(topic))
                _subscriptions.Remove(topic);
            else
                return Task.CompletedTask;
            return SubmitSubscriptionsAsync();
        }

        /// <summary>
        /// Unsubscribes from all real-time events for topics that start with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix to match against topic names</param>
        /// <returns>A task representing the asynchronous unsubscription operation</returns>
        public async Task UnsubscribeByPrefixAsync(string prefix)
        {
            var subscriptionsToRemove = _subscriptions.Keys.Where(k => k.StartsWith(prefix)).ToList();
            if (subscriptionsToRemove.Any())
            {
                foreach (var subs in subscriptionsToRemove)
                    _subscriptions.Remove(subs);

                await SubmitSubscriptionsAsync();
            }
        }

        /// <summary>
        /// Unsubscribes a specific listener from a specific topic.
        /// If no listeners remain for the topic, the topic subscription is completely removed.
        /// </summary>
        /// <param name="topic">The topic to unsubscribe from</param>
        /// <param name="listener">The specific listener callback to remove</param>
        /// <returns>A task representing the asynchronous unsubscription operation</returns>
        public async Task UnsubscribeByTopicAndListenerAsync(string topic, Func<SseMessage, Task> listener)
        {
            if (!_subscriptions.ContainsKey(topic))
                return;

            var listeners = _subscriptions[topic];
            if (listeners.Remove(listener) && !listeners.Any())
                await UnsubscribeAsync(topic);
        }

        private async Task SubmitSubscriptionsAsync()
        {
            if (!_subscriptions.Any())
                SseClient.Disconnect();
            else
            {
                await SseClient.EnsureIsConnectedAsync();
                Dictionary<string, object> body = new()
                {
                    { "clientId", SseClient.Id! },
                    { "subscriptions", _subscriptions.Keys.ToList() }
                };

                await _client.SendAsync(BasePath(), HttpMethod.Post, body: body);
            }
        }
    }
}
