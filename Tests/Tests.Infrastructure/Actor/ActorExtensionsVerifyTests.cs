using FluentAssertions;
using PlayGround.Infrastructure.Actor;

namespace PlayGround.Tests.Infrastructure.Actor;

public class ActorExtensionsVerifyTests
{
    #region Verify<T> (ActorMessage 기본)

    [Fact]
    public void Verify_WithValidMessage_ShouldReturnTrue()
    {
        // Arrange
        var message = new ActorMessage();

        // Act
        var result = message.Verify();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_WithNull_ShouldReturnFalse()
    {
        // Arrange
        ActorMessage? message = null;

        // Act
        var result = message.Verify();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Verify<TRequest> (ActorMessage<TRequest>)

    [Fact]
    public void VerifyT_WithRequestData_ShouldReturnVerifiedAndData()
    {
        // Arrange
        var message = new ActorMessage<TestRequest>
        {
            RequestData = new TestRequest { Name = "Son" }
        };

        // Act
        var (verified, request) = message.Verify();

        // Assert
        verified.Should().BeTrue();
        request.Name.Should().Be("Son");
    }

    [Fact]
    public void VerifyT_WithNullRequestData_ShouldReturnFalseAndNewInstance()
    {
        // Arrange
        var message = new ActorMessage<TestRequest> { RequestData = null };

        // Act
        var (verified, request) = message.Verify();

        // Assert
        verified.Should().BeFalse();
        request.Should().NotBeNull();
        request.Name.Should().BeNull();
    }

    [Fact]
    public void VerifyT_WithNullMessage_ShouldReturnFalseAndNewInstance()
    {
        // Arrange
        ActorMessage<TestRequest>? message = null;

        // Act
        var (verified, request) = message.Verify();

        // Assert
        verified.Should().BeFalse();
        request.Should().NotBeNull();
    }

    #endregion

    #region Verify<TRequest, TResult> (ActorMessage<TRequest, TResult>)

    [Fact]
    public void VerifyTR_WithRequestData_ShouldReturnVerifiedAndData()
    {
        // Arrange
        var message = new ActorMessage<TestRequest, TestResult>
        {
            RequestData = new TestRequest { Name = "Kane" }
        };

        // Act
        var (verified, request) = message.Verify();

        // Assert
        verified.Should().BeTrue();
        request.Name.Should().Be("Kane");
    }

    [Fact]
    public void VerifyTR_WithNullRequestData_ShouldReturnFalseAndNewInstance()
    {
        // Arrange
        var message = new ActorMessage<TestRequest, TestResult> { RequestData = null };

        // Act
        var (verified, request) = message.Verify();

        // Assert
        verified.Should().BeFalse();
        request.Should().NotBeNull();
    }

    [Fact]
    public void VerifyTR_WithNullMessage_ShouldReturnFalseAndNewInstance()
    {
        // Arrange
        ActorMessage<TestRequest, TestResult>? message = null;

        // Act
        var (verified, request) = message.Verify();

        // Assert
        verified.Should().BeFalse();
        request.Should().NotBeNull();
    }

    #endregion

    #region Test Models

    public class TestRequest
    {
        public string? Name { get; set; }
    }

    public class TestResult
    {
        public int Score { get; set; }
    }

    #endregion
}
