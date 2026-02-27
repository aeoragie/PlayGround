using FluentAssertions;
using Xunit;
using PlayGround.Infrastructure.Store;

namespace PlayGround.Tests.Infrastructure.Store;

public class RedisResultTests
{
    #region Ok

    [Fact]
    public void Ok_WithValue_ShouldBeSuccessWithValue()
    {
        // Arrange & Act
        var result = RedisResult<string>.Ok("hello");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be("hello");
        result.Message.Should().BeNull();
    }

    [Fact]
    public void Ok_WithNull_ShouldBeSuccessWithoutValue()
    {
        // Arrange & Act
        var result = RedisResult<string>.Ok(null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeFalse();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Ok_WithIntValue_ShouldRetainValue()
    {
        // Arrange & Act
        var result = RedisResult<int>.Ok(42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    #endregion

    #region Empty

    [Fact]
    public void Empty_ShouldBeSuccessWithoutValue()
    {
        // Arrange & Act
        var result = RedisResult<string>.Empty();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Message.Should().BeNull();
    }

    [Fact]
    public void Empty_WithValueType_ShouldReturnDefault()
    {
        // Arrange & Act
        var result = RedisResult<int>.Empty();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeFalse();
        result.Value.Should().Be(default(int));
    }

    #endregion

    #region Fail

    [Fact]
    public void Fail_WithoutException_ShouldBeFailedWithoutError()
    {
        // Arrange & Act
        var result = RedisResult<string>.Fail();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasValue.Should().BeFalse();
        result.Message.Should().BeNull();
    }

    [Fact]
    public void Fail_WithException_ShouldBeFailedWithError()
    {
        // Arrange
        var exception = new InvalidOperationException("connection lost");

        // Act
        var result = RedisResult<string>.Fail(exception);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasValue.Should().BeFalse();
        result.Message.Should().BeSameAs(exception.Message);
        result.Message.Should().Be("connection lost");
    }

    #endregion
}
