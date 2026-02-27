using Akka.Routing;

namespace PlayGround.Infrastructure.Actor
{
    /// <summary>
    /// 액터 메시지 결과 코드
    /// </summary>
    public enum ActorResultCode
    {
        Success = 0,
        Error,
        Timeout,
        ResultDataNull,
    }

    /// <summary>
    /// 데이터 없는 액터 메시지 (알림, fire-and-forget)
    /// </summary>
    public class ActorMessage : IConsistentHashable, IDisposable
    {
        public ActorResultCode ResultCode { get; set; } = ActorResultCode.Success;
        public object? ConsistentHashKey { get; set; }

        public bool IsSuccess => ResultCode == ActorResultCode.Success;

        public ActorMessage SetResultCode(ActorResultCode code)
        {
            ResultCode = code;
            return this;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 요청 데이터만 있는 액터 메시지
    /// </summary>
    public class ActorMessage<TRequest> : IConsistentHashable, IDisposable
    {
        public ActorResultCode ResultCode { get; set; } = ActorResultCode.Success;
        public object? ConsistentHashKey { get; set; }
        public TRequest? RequestData { get; set; }

        public bool IsSuccess => ResultCode == ActorResultCode.Success;

        public ActorMessage<TRequest> SetResultCode(ActorResultCode code)
        {
            ResultCode = code;
            return this;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 요청 + 응답 데이터가 있는 액터 메시지
    /// </summary>
    public class ActorMessage<TRequest, TResult> : IConsistentHashable, IDisposable
    {
        public ActorResultCode ResultCode { get; set; } = ActorResultCode.Success;
        public string ResultMessage { get; set; } = string.Empty;
        public object? ConsistentHashKey { get; set; }

        public TRequest? RequestData { get; set; }
        public TResult? ResultData { get; set; }

        public bool IsSuccess => ResultCode == ActorResultCode.Success;

        public ActorMessage<TRequest, TResult> SetResultCode(ActorResultCode code)
        {
            ResultCode = code;
            return this;
        }

        public ActorMessage<TRequest, TResult> SetResult(ActorResultCode code, string message = "")
        {
            ResultCode = code;
            ResultMessage = message;
            return this;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
