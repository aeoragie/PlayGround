using Akka.Actor;

namespace PlayGround.Infrastructure.Actor
{
    /// <summary>
    /// IActorRef 래퍼 (Send/SendAsync 편의 메서드 제공)
    /// </summary>
    public class ActorRef(IActorRef actorRef, string actorName)
    {
        public string Name { get; } = actorName;
        public IActorRef Raw { get; } = actorRef;
        public long RouterHashKey { get; set; }

        #region Fire-and-Forget

        public void Send(ActorMessage message)
        {
            ApplyHashKey(message);
            Raw.Tell(message);
        }

        public void Send<TRequest>(ActorMessage<TRequest> message)
        {
            ApplyHashKey(message);
            Raw.Tell(message);
        }

        #endregion

        #region Request-Response (Async)

        public async Task<ActorMessage> SendAsync(ActorMessage message)
        {
            ApplyHashKey(message);
            return await Raw.SendAsync(message);
        }

        public async Task<ActorMessage<TRequest>> SendAsync<TRequest>(ActorMessage<TRequest> message)
        {
            ApplyHashKey(message);
            return await Raw.SendAsync(message);
        }

        public async Task<ActorMessage<TRequest, TResult>> SendAsync<TRequest, TResult>(
            ActorMessage<TRequest, TResult> message)
            where TResult : class, new()
        {
            ApplyHashKey(message);
            return await Raw.SendAsync(message);
        }

        #endregion

        private void ApplyHashKey(ActorMessage message)
        {
            if (RouterHashKey != 0)
            {
                message.ConsistentHashKey = RouterHashKey;
            }
        }

        private void ApplyHashKey<TRequest>(ActorMessage<TRequest> message)
        {
            if (RouterHashKey != 0)
            {
                message.ConsistentHashKey = RouterHashKey;
            }
        }

        private void ApplyHashKey<TRequest, TResult>(ActorMessage<TRequest, TResult> message)
        {
            if (RouterHashKey != 0)
            {
                message.ConsistentHashKey = RouterHashKey;
            }
        }
    }
}
