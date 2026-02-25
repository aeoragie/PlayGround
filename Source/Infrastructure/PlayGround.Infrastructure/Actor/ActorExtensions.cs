using Akka.Actor;

namespace PlayGround.Infrastructure.Actor
{
    /// <summary>
    /// IActorRef 확장 메서드 (Response, SendAsync, Verify)
    /// </summary>
    public static class ActorExtensions
    {
        #region Response (액터 내부에서 Sender에게 응답)

        public static void Response(this IActorRef sender, ActorMessage message)
        {
            sender.Tell(message);
        }

        public static void Response<TRequest>(this IActorRef sender, ActorMessage<TRequest> message)
        {
            sender.Tell(message);
        }

        public static void Response<TRequest, TResult>(this IActorRef sender, ActorMessage<TRequest, TResult> message)
        {
            sender.Tell(message);
        }

        #endregion

        #region SendAsync (Ask 패턴 래핑)

        public static async Task<ActorMessage> SendAsync(this IActorRef actor, ActorMessage message)
        {
            var result = await actor.Ask<ActorMessage>(message);
            return result ?? message.SetResultCode(ActorResultCode.Error);
        }

        public static async Task<ActorMessage<TRequest>> SendAsync<TRequest>(
            this IActorRef actor, ActorMessage<TRequest> message)
        {
            var result = await actor.Ask<ActorMessage<TRequest>>(message);
            return result ?? message.SetResultCode(ActorResultCode.Error);
        }

        public static async Task<ActorMessage<TRequest, TResult>> SendAsync<TRequest, TResult>(
            this IActorRef actor, ActorMessage<TRequest, TResult> message)
            where TResult : class, new()
        {
            var result = await actor.Ask<ActorMessage<TRequest, TResult>>(message);
            if (result?.ResultData == null)
            {
                message.ResultData = new TResult();
                return message.SetResultCode(ActorResultCode.ResultDataNull);
            }
            return result;
        }

        #endregion

        #region Verify (메시지 검증)

        public static bool Verify<T>(this T? message) where T : ActorMessage
        {
            return message != null;
        }

        public static (bool Verified, TRequest Request) Verify<TRequest>(
            this ActorMessage<TRequest>? message) where TRequest : class, new()
        {
            if (message?.RequestData == null)
            {
                return (false, new TRequest());
            }
            return (true, message.RequestData);
        }

        public static (bool Verified, TRequest Request) Verify<TRequest, TResult>(
            this ActorMessage<TRequest, TResult>? message) where TRequest : class, new()
        {
            if (message?.RequestData == null)
            {
                return (false, new TRequest());
            }
            return (true, message.RequestData);
        }

        #endregion
    }
}
