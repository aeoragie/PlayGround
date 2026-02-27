using FluentAssertions;
using Moq;
using Xunit;
using StackExchange.Redis;
using PlayGround.Infrastructure.Store;

namespace PlayGround.Tests.Infrastructure.Store;

public class RedisSessionTests
{
    private readonly Mock<IConnectionMultiplexer> MockMultiplexer;
    private readonly Mock<IDatabase> MockDatabase;
    private readonly RedisSession Session;

    public RedisSessionTests()
    {
        MockMultiplexer = new Mock<IConnectionMultiplexer>();
        MockDatabase = new Mock<IDatabase>();
        MockMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(MockDatabase.Object);
        MockMultiplexer.Setup(m => m.IsConnected).Returns(true);

        Session = new RedisSession(MockMultiplexer.Object);
    }

    #region Constructor

    [Fact]
    public void Constructor_WithNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new RedisSession(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ShouldExposeDatabase()
    {
        // Assert
        Session.Database.Should().BeSameAs(MockDatabase.Object);
    }

    [Fact]
    public void IsConnected_ShouldDelegateToMultiplexer()
    {
        // Assert
        Session.IsConnected.Should().BeTrue();

        // Arrange
        MockMultiplexer.Setup(m => m.IsConnected).Returns(false);

        // Assert
        Session.IsConnected.Should().BeFalse();
    }

    #endregion

    #region String Get/Set

    [Fact]
    public async Task TryStringGetAsync_WithValue_ShouldReturnOkAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)"test-value");

        // Act
        var result = await Session.TryStringGetAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be("test-value");
    }

    [Fact]
    public async Task TryStringGetAsync_WithNullValue_ShouldReturnEmptyAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await Session.TryStringGetAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task TryStringGetAsync_WhenDisconnected_ShouldReturnFailAsync()
    {
        // Arrange
        MockMultiplexer.Setup(m => m.IsConnected).Returns(false);

        // Act
        var result = await Session.TryStringGetAsync("key1");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task TryStringGetAsync_WhenException_ShouldReturnFailWithErrorAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "timeout"));

        // Act
        var result = await Session.TryStringGetAsync("key1");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().BeOfType<RedisConnectionException>();
    }

    [Fact]
    public async Task TryStringSetAsync_ShouldReturnOkAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await Session.TryStringSetAsync("key1", "value1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task TryStringSetAsync_WithExpiry_ShouldPassExpiryAsync()
    {
        // Arrange
        var expiry = TimeSpan.FromMinutes(10);
        MockDatabase.Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), expiry,
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await Session.TryStringSetAsync("key1", "value1", expiry);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task TryStringSetAsync_WhenDisconnected_ShouldReturnFailAsync()
    {
        // Arrange
        MockMultiplexer.Setup(m => m.IsConnected).Returns(false);

        // Act
        var result = await Session.TryStringSetAsync("key1", "value1");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region Generic Get/Set (JSON)

    [Fact]
    public async Task TryGetAsync_WithSerializedValue_ShouldDeserializeAsync()
    {
        // Arrange
        var json = System.Text.Json.JsonSerializer.Serialize(new TestData { Id = 1, Name = "Player" });
        MockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)json);

        // Act
        var result = await Session.TryGetAsync<TestData>("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeTrue();
        result.Value!.Id.Should().Be(1);
        result.Value.Name.Should().Be("Player");
    }

    [Fact]
    public async Task TrySetAsync_ShouldSerializeAndStoreAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await Session.TrySetAsync("key1", new TestData { Id = 1, Name = "Player" });

        // Assert
        result.IsSuccess.Should().BeTrue();
        MockDatabase.Verify(db => db.StringSetAsync(
            (RedisKey)"key1",
            It.Is<RedisValue>(v => v.ToString().Contains("Player")),
            null, It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    #endregion

    #region Hash

    [Fact]
    public async Task TryHashSetAsync_ShouldReturnOkAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.HashSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await Session.TryHashSetAsync("hash1", "field1", "value1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task TryHashGetAsync_WithValue_ShouldDeserializeAsync()
    {
        // Arrange
        var json = System.Text.Json.JsonSerializer.Serialize("hello");
        MockDatabase.Setup(db => db.HashGetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)json);

        // Act
        var result = await Session.TryHashGetAsync<string, string>("hash1", "field1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public async Task TryHashGetAsync_WithNullValue_ShouldReturnEmptyAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.HashGetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await Session.TryHashGetAsync<string, string>("hash1", "field1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task TryHashAllGetAsync_ShouldReturnDictionaryAsync()
    {
        // Arrange
        var entries = new[]
        {
            new HashEntry("k1", System.Text.Json.JsonSerializer.Serialize(100)),
            new HashEntry("k2", System.Text.Json.JsonSerializer.Serialize(200))
        };
        MockDatabase.Setup(db => db.HashGetAllAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(entries);

        // Act
        var result = await Session.TryHashAllGetAsync<int>("hash1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value!["k1"].Should().Be(100);
        result.Value["k2"].Should().Be(200);
    }

    [Fact]
    public async Task TryHashSetAsync_WhenDisconnected_ShouldReturnFailAsync()
    {
        // Arrange
        MockMultiplexer.Setup(m => m.IsConnected).Returns(false);

        // Act
        var result = await Session.TryHashSetAsync("hash1", "field1", "value1");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region Key

    [Fact]
    public async Task TryKeyExistsAsync_WhenExists_ShouldReturnTrueAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await Session.TryKeyExistsAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task TryKeyDeleteAsync_ShouldReturnResultAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await Session.TryKeyDeleteAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task TryGetExpiryRemainingAsync_ShouldReturnTtlAsync()
    {
        // Arrange
        var ttl = TimeSpan.FromMinutes(5);
        MockDatabase.Setup(db => db.KeyTimeToLiveAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(ttl);

        // Act
        var result = await Session.TryGetExpiryRemainingAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ttl);
    }

    [Fact]
    public async Task SetExpiryAsync_ShouldDelegateToDatabaseAsync()
    {
        // Arrange
        var expiry = TimeSpan.FromHours(1);
        MockDatabase.Setup(db => db.KeyExpireAsync(
                It.IsAny<RedisKey>(), expiry, It.IsAny<ExpireWhen>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await Session.SetExpiryAsync("key1", expiry);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Ping

    [Fact]
    public async Task PingAsync_WhenHealthy_ShouldReturnTrueAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.PingAsync(It.IsAny<CommandFlags>()))
            .ReturnsAsync(TimeSpan.FromMilliseconds(5));

        // Act
        var result = await Session.PingAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PingAsync_WhenSlow_ShouldReturnFalseAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.PingAsync(It.IsAny<CommandFlags>()))
            .ReturnsAsync(TimeSpan.FromMilliseconds(1500));

        // Act
        var result = await Session.PingAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task PingAsync_WhenException_ShouldReturnFalseAsync()
    {
        // Arrange
        MockDatabase.Setup(db => db.PingAsync(It.IsAny<CommandFlags>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "down"));

        // Act
        var result = await Session.PingAsync();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Dispose

    [Fact]
    public async Task DisposeAsync_ShouldNotThrowAsync()
    {
        // Act & Assert
        await Session.Invoking(s => s.DisposeAsync().AsTask()).Should().NotThrowAsync();
    }

    #endregion

    #region Test Models

    public class TestData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    #endregion
}
