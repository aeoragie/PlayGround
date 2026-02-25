using FluentAssertions;
using PlayGround.Infrastructure.Logging;

namespace PlayGround.Tests.Infrastructure.Logging;

public class LoggingConfigTests
{
    [Fact]
    public void Section_ShouldBeLoggingConfig()
    {
        // Assert
        LoggingConfig.Section.Should().Be("LoggingConfig");
    }

    [Fact]
    public void Default_LogLevel_ShouldBeInfo()
    {
        // Arrange & Act
        var config = new LoggingConfig();

        // Assert
        config.LogLevel.Should().Be("Info");
    }

    [Fact]
    public void Default_EnableFileLogging_ShouldBeTrue()
    {
        // Arrange & Act
        var config = new LoggingConfig();

        // Assert
        config.EnableFileLogging.Should().BeTrue();
    }

    [Fact]
    public void Default_EnableConsoleLogging_ShouldBeTrue()
    {
        // Arrange & Act
        var config = new LoggingConfig();

        // Assert
        config.EnableConsoleLogging.Should().BeTrue();
    }

    [Fact]
    public void Default_MaxArchiveFiles_ShouldBe30()
    {
        // Arrange & Act
        var config = new LoggingConfig();

        // Assert
        config.MaxArchiveFiles.Should().Be(30);
    }

    [Fact]
    public void SetProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var config = new LoggingConfig
        {
            LogLevel = "Warn",
            EnableFileLogging = false,
            EnableConsoleLogging = false,
            MaxArchiveFiles = 10
        };

        // Assert
        config.LogLevel.Should().Be("Warn");
        config.EnableFileLogging.Should().BeFalse();
        config.EnableConsoleLogging.Should().BeFalse();
        config.MaxArchiveFiles.Should().Be(10);
    }
}
