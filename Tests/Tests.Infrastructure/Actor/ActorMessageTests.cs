using FluentAssertions;
using PlayGround.Infrastructure.Actor;

namespace PlayGround.Tests.Infrastructure.Actor;

public class ActorMessageTests
{
    #region ActorMessage (기본)

    [Fact]
    public void ActorMessage_Default_ShouldBeSuccess()
    {
        // Arrange & Act
        var message = new ActorMessage();

        // Assert
        message.ResultCode.Should().Be(ActorResultCode.Success);
        message.IsSuccess.Should().BeTrue();
        message.ConsistentHashKey.Should().BeNull();
    }

    [Fact]
    public void ActorMessage_SetResultCode_ShouldUpdateAndReturnSelf()
    {
        // Arrange
        var message = new ActorMessage();

        // Act
        var returned = message.SetResultCode(ActorResultCode.Error);

        // Assert
        returned.Should().BeSameAs(message);
        message.ResultCode.Should().Be(ActorResultCode.Error);
        message.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void ActorMessage_Dispose_ShouldNotThrow()
    {
        // Arrange
        var message = new ActorMessage();

        // Act & Assert
        message.Invoking(m => m.Dispose()).Should().NotThrow();
    }

    #endregion

    #region ActorMessage<TRequest>

    [Fact]
    public void ActorMessageT_Default_ShouldHaveNullRequestData()
    {
        // Arrange & Act
        var message = new ActorMessage<string>();

        // Assert
        message.IsSuccess.Should().BeTrue();
        message.RequestData.Should().BeNull();
    }

    [Fact]
    public void ActorMessageT_WithRequestData_ShouldRetainData()
    {
        // Arrange & Act
        var message = new ActorMessage<string> { RequestData = "test-request" };

        // Assert
        message.RequestData.Should().Be("test-request");
    }

    [Fact]
    public void ActorMessageT_SetResultCode_ShouldUpdateAndReturnSelf()
    {
        // Arrange
        var message = new ActorMessage<int>();

        // Act
        var returned = message.SetResultCode(ActorResultCode.Timeout);

        // Assert
        returned.Should().BeSameAs(message);
        message.ResultCode.Should().Be(ActorResultCode.Timeout);
        message.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region ActorMessage<TRequest, TResult>

    [Fact]
    public void ActorMessageTR_Default_ShouldHaveNullData()
    {
        // Arrange & Act
        var message = new ActorMessage<string, List<int>>();

        // Assert
        message.IsSuccess.Should().BeTrue();
        message.RequestData.Should().BeNull();
        message.ResultData.Should().BeNull();
        message.ResultMessage.Should().BeEmpty();
    }

    [Fact]
    public void ActorMessageTR_SetResult_ShouldUpdateCodeAndMessage()
    {
        // Arrange
        var message = new ActorMessage<string, List<int>>();

        // Act
        var returned = message.SetResult(ActorResultCode.Error, "something failed");

        // Assert
        returned.Should().BeSameAs(message);
        message.ResultCode.Should().Be(ActorResultCode.Error);
        message.ResultMessage.Should().Be("something failed");
        message.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void ActorMessageTR_WithData_ShouldRetainBothRequestAndResult()
    {
        // Arrange & Act
        var message = new ActorMessage<string, List<int>>
        {
            RequestData = "query",
            ResultData = new List<int> { 1, 2, 3 }
        };

        // Assert
        message.RequestData.Should().Be("query");
        message.ResultData.Should().HaveCount(3);
    }

    [Fact]
    public void ActorMessageTR_ConsistentHashKey_ShouldBeSettable()
    {
        // Arrange
        var message = new ActorMessage<string, string>();

        // Act
        message.ConsistentHashKey = 12345L;

        // Assert
        message.ConsistentHashKey.Should().Be(12345L);
    }

    #endregion

    #region ActorResultCode

    [Theory]
    [InlineData(ActorResultCode.Success, true)]
    [InlineData(ActorResultCode.Error, false)]
    [InlineData(ActorResultCode.Timeout, false)]
    [InlineData(ActorResultCode.ResultDataNull, false)]
    public void IsSuccess_ShouldReflectResultCode(ActorResultCode code, bool expectedSuccess)
    {
        // Arrange
        var message = new ActorMessage();

        // Act
        message.SetResultCode(code);

        // Assert
        message.IsSuccess.Should().Be(expectedSuccess);
    }

    #endregion
}
