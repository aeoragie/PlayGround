using FluentAssertions;
using Moq;
using PlayGround.Infrastructure.Store;
using StackExchange.Redis;

namespace PlayGround.Tests.Infrastructure.Store;

public class RedisSessionTests
{
    private readonly Mock<IConnectionMultiplexer> mMockMultiplexer;
    private readonly Mock<IDatabase> mMockDatabase;
    private readonly RedisSession mSession;

    public RedisSessionTests()
    {
        mMockMultiplexer = new Mock<IConnectionMultiplexer>();
        mMockDatabase = new Mock<IDatabase>();
        mMockMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(mMockDatabase.Object);
        mMockMultiplexer.Setup(m => m.IsConnected).Returns(true);

        mSession = new RedisSession(mMockMultiplexer.Object);
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
        mSession.Database.Should().BeSameAs(mMockDatabase.Object);
    }

    [Fact]
    public void IsConnected_ShouldDelegateToMultiplexer()
    {
        // Assert
        mSession.IsConnected.Should().BeTrue();

        // Arrange
        mMockMultiplexer.Setup(m => m.IsConnected).Returns(false);

        // Assert
        mSession.IsConnected.Should().BeFalse();
    }

    #endregion

    #region String Get/Set

    [Fact]
    public async Task TryStringGetAsync_WithValue_ShouldReturnOk()
    {
        // Arrange
        mMockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)"test-value");

        // Act
        var result = await mSession.TryStringGetAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be("test-value");
    }

    [Fact]
    public async Task TryStringGetAsync_WithNullValue_ShouldReturnEmpty()
    {
        // Arrange
        mMockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await mSession.TryStringGetAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task TryStringGetAsync_WhenDisconnected_ShouldReturnFail()
    {
        // Arrange
        mMockMultiplexer.Setup(m => m.IsConnected).Returns(false);

        // Act
        var result = await mSession.TryStringGetAsync("key1");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task TryStringGetAsync_WhenException_ShouldReturnFailWithError()
    {
        // Arrange
        mMockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "timeout"));

        // Act
        var result = await mSession.TryStringGetAsync("key1");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeOfType<RedisConnectionException>();
    }

    [Fact]
    public async Task TryStringSetAsync_ShouldReturnOk()
    {
        // Arrange
        mMockDatabase.Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await mSession.TryStringSetAsync("key1", "value1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task TryStringSetAsync_WithExpiry_ShouldPassExpiry()
    {
        // Arrange
        var expiry = TimeSpan.FromMinutes(10);
        mMockDatabase.Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), expiry,
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await mSession.TryStringSetAsync("key1", "value1", expiry);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task TryStringSetAsync_WhenDisconnected_ShouldReturnFail()
    {
        // Arrange
        mMockMultiplexer.Setup(m => m.IsConnected).Returns(false);

        // Act
        var result = await mSession.TryStringSetAsync("key1", "value1");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region Generic Get/Set (JSON)

    [Fact]
    public async Task TryGetAsync_WithSerializedValue_ShouldDeserialize()
    {
        // Arrange
        var json = System.Text.Json.JsonSerializer.Serialize(new TestData { Id = 1, Name = "Player" });
        mMockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)json);

        // Act
        var result = await mSession.TryGetAsync<TestData>("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeTrue();
        result.Value!.Id.Should().Be(1);
        result.Value.Name.Should().Be("Player");
    }

    [Fact]
    public async Task TrySetAsync_ShouldSerializeAndStore()
    {
        // Arrange
        mMockDatabase.Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await mSession.TrySetAsync("key1", new TestData { Id = 1, Name = "Player" });

        // Assert
        result.IsSuccess.Should().BeTrue();
        mMockDatabase.Verify(db => db.StringSetAsync(
            (RedisKey)"key1",
            It.Is<RedisValue>(v => v.ToString().Contains("Player")),
            null, It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    #endregion

    #region Hash

    [Fact]
    public async Task TryHashSetAsync_ShouldReturnOk()
    {
        // Arrange
        mMockDatabase.Setup(db => db.HashSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await mSession.TryHashSetAsync("hash1", "field1", "value1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task TryHashGetAsync_WithValue_ShouldDeserialize()
    {
        // Arrange
        var json = System.Text.Json.JsonSerializer.Serialize("hello");
        mMockDatabase.Setup(db => db.HashGetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)json);

        // Act
        var result = await mSession.TryHashGetAsync<string, string>("hash1", "field1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public async Task TryHashGetAsync_WithNullValue_ShouldReturnEmpty()
    {
        // Arrange
        mMockDatabase.Setup(db => db.HashGetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        var result = await mSession.TryHashGetAsync<string, string>("hash1", "field1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task TryHashAllGetAsync_ShouldReturnDictionary()
    {
        // Arrange
        var entries = new[]
        {
            new HashEntry("k1", System.Text.Json.JsonSerializer.Serialize(100)),
            new HashEntry("k2", System.Text.Json.JsonSerializer.Serialize(200))
        };
        mMockDatabase.Setup(db => db.HashGetAllAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(entries);

        // Act
        var result = await mSession.TryHashAllGetAsync<int>("hash1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value!["k1"].Should().Be(100);
        result.Value["k2"].Should().Be(200);
    }

    [Fact]
    public async Task TryHashSetAsync_WhenDisconnected_ShouldReturnFail()
    {
        // Arrange
        mMockMultiplexer.Setup(m => m.IsConnected).Returns(false);

        // Act
        var result = await mSession.TryHashSetAsync("hash1", "field1", "value1");

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region Key

    [Fact]
    public async Task TryKeyExistsAsync_WhenExists_ShouldReturnTrue()
    {
        // Arrange
        mMockDatabase.Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await mSession.TryKeyExistsAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task TryKeyDeleteAsync_ShouldReturnResult()
    {
        // Arrange
        mMockDatabase.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await mSession.TryKeyDeleteAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task TryGetExpiryRemainingAsync_ShouldReturnTtl()
    {
        // Arrange
        var ttl = TimeSpan.FromMinutes(5);
        mMockDatabase.Setup(db => db.KeyTimeToLiveAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(ttl);

        // Act
        var result = await mSession.TryGetExpiryRemainingAsync("key1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ttl);
    }

    [Fact]
    public async Task SetExpiryAsync_ShouldDelegateToDatabase()
    {
        // Arrange
        var expiry = TimeSpan.FromHours(1);
        mMockDatabase.Setup(db => db.KeyExpireAsync(
                It.IsAny<RedisKey>(), expiry, It.IsAny<ExpireWhen>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await mSession.SetExpiryAsync("key1", expiry);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Ping

    [Fact]
    public async Task PingAsync_WhenHealthy_ShouldReturnTrue()
    {
        // Arrange
        mMockDatabase.Setup(db => db.PingAsync(It.IsAny<CommandFlags>()))
            .ReturnsAsync(TimeSpan.FromMilliseconds(5));

        // Act
        var result = await mSession.PingAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PingAsync_WhenSlow_ShouldReturnFalse()
    {
        // Arrange
        mMockDatabase.Setup(db => db.PingAsync(It.IsAny<CommandFlags>()))
            .ReturnsAsync(TimeSpan.FromMilliseconds(1500));

        // Act
        var result = await mSession.PingAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task PingAsync_WhenException_ShouldReturnFalse()
    {
        // Arrange
        mMockDatabase.Setup(db => db.PingAsync(It.IsAny<CommandFlags>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "down"));

        // Act
        var result = await mSession.PingAsync();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Dispose

    [Fact]
    public async Task DisposeAsync_ShouldNotThrow()
    {
        // Act & Assert
        await mSession.Invoking(s => s.DisposeAsync().AsTask()).Should().NotThrowAsync();
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
