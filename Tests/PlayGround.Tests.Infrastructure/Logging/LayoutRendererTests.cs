using System.Text;
using FluentAssertions;
using NLog;
using PlayGround.Infrastructure.Logging.Render;

namespace PlayGround.Tests.Infrastructure.Logging;

public class LayoutRendererTests
{
    #region ArchiveDateLayoutRenderer

    [Fact]
    public void ArchiveDate_ShouldFormat_yyyyMMdd()
    {
        // Arrange
        var renderer = new ArchiveDateLayoutRenderer();
        var logEvent = LogEventInfo.Create(LogLevel.Info, "test", "message");
        logEvent.TimeStamp = new DateTime(2026, 2, 25, 14, 30, 0);
        var sb = new StringBuilder();

        // Act
        renderer.RenderAppendBuilder(logEvent, sb);

        // Assert
        sb.ToString().Should().Be("2026_02_25");
    }

    [Fact]
    public void ArchiveDate_DifferentDate_ShouldFormat()
    {
        // Arrange
        var renderer = new ArchiveDateLayoutRenderer();
        var logEvent = LogEventInfo.Create(LogLevel.Info, "test", "message");
        logEvent.TimeStamp = new DateTime(2025, 1, 5, 0, 0, 0);
        var sb = new StringBuilder();

        // Act
        renderer.RenderAppendBuilder(logEvent, sb);

        // Assert
        sb.ToString().Should().Be("2025_01_05");
    }

    #endregion

    #region ThreadIdLayoutRenderer

    [Fact]
    public void ThreadId_ShouldBe4DigitZeroPadded()
    {
        // Arrange
        var renderer = new ThreadIdLayoutRenderer();
        var logEvent = LogEventInfo.Create(LogLevel.Info, "test", "message");
        var sb = new StringBuilder();

        // Act
        renderer.RenderAppendBuilder(logEvent, sb);

        // Assert
        var result = sb.ToString();
        result.Should().HaveLength(4);
        result.Should().MatchRegex(@"^\d{4}$");
    }

    [Fact]
    public void ThreadId_ShouldMatchCurrentThread()
    {
        // Arrange
        var renderer = new ThreadIdLayoutRenderer();
        var logEvent = LogEventInfo.Create(LogLevel.Info, "test", "message");
        var sb = new StringBuilder();
        var expectedId = Environment.CurrentManagedThreadId.ToString().PadLeft(4, '0');

        // Act
        renderer.RenderAppendBuilder(logEvent, sb);

        // Assert
        sb.ToString().Should().Be(expectedId);
    }

    #endregion
}
