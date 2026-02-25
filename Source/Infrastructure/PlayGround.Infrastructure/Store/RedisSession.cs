using System.Diagnostics;
using System.Text.Json;
using NLog;
using StackExchange.Redis;

namespace PlayGround.Infrastructure.Store
{
    /// <summary>
    /// IRedisSession 구현 (IDatabase 래퍼, 도메인 세션으로 확장 가능)
    /// </summary>
    public class RedisSession : IRedisSession
    {
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConnectionMultiplexer mMultiplexer;
        protected readonly IDatabase mDatabase;

        public IDatabase Database => mDatabase;
        public bool IsConnected => mMultiplexer.IsConnected;

        public RedisSession(IConnectionMultiplexer multiplexer, int databaseId = 0)
        {
            Debug.Assert(multiplexer != null, "ConnectionMultiplexer cannot be null");
            mMultiplexer = multiplexer ?? throw new ArgumentNullException(nameof(multiplexer));
            mDatabase = mMultiplexer.GetDatabase(databaseId);
        }

        #region String

        public async Task<RedisResult<string>> TryStringGetAsync(string key)
        {
            try
            {
                if (!IsConnected)
                {
                    return RedisResult<string>.Fail();
                }

                var value = await mDatabase.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                {
                    return RedisResult<string>.Empty();
                }

                return RedisResult<string>.Ok(value!);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryStringGetAsync failed: {Key}", key);
                return RedisResult<string>.Fail(ex);
            }
        }

        public async Task<RedisResult<T>> TryGetAsync<T>(string key)
        {
            try
            {
                var result = await TryStringGetAsync(key);
                if (!result.IsSuccess || result.Value is null)
                {
                    return RedisResult<T>.Empty();
                }

                return RedisResult<T>.Ok(JsonSerializer.Deserialize<T>(result.Value));
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryGetAsync failed: {Key}", key);
                return RedisResult<T>.Fail(ex);
            }
        }

        public async Task<RedisResult<bool>> TryStringSetAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                if (!IsConnected)
                {
                    return RedisResult<bool>.Fail();
                }

                var result = await mDatabase.StringSetAsync(key, value, expiry);
                return RedisResult<bool>.Ok(result);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryStringSetAsync failed: {Key}", key);
                return RedisResult<bool>.Fail(ex);
            }
        }

        public async Task<RedisResult<bool>> TrySetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                return await TryStringSetAsync(key, json, expiry);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TrySetAsync failed: {Key}", key);
                return RedisResult<bool>.Fail(ex);
            }
        }

        #endregion

        #region Hash

        public async Task<RedisResult<bool>> TryHashSetAsync<TField, TValue>(string key, TField hashField, TValue value)
        {
            try
            {
                if (!IsConnected)
                {
                    return RedisResult<bool>.Fail();
                }

                var field = JsonSerializer.Serialize(hashField);
                var json = JsonSerializer.Serialize(value);
                await mDatabase.HashSetAsync(key, field, json);
                return RedisResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryHashSetAsync failed: {Key}:{Field}", key, hashField);
                return RedisResult<bool>.Fail(ex);
            }
        }

        public async Task<RedisResult<TValue>> TryHashGetAsync<TField, TValue>(string key, TField hashField)
        {
            try
            {
                if (!IsConnected)
                {
                    return RedisResult<TValue>.Fail();
                }

                var field = JsonSerializer.Serialize(hashField);
                var value = await mDatabase.HashGetAsync(key, field);
                if (value.IsNull)
                {
                    return RedisResult<TValue>.Empty();
                }

                var deserialized = JsonSerializer.Deserialize<TValue>(value!);
                return deserialized is null
                    ? RedisResult<TValue>.Empty()
                    : RedisResult<TValue>.Ok(deserialized);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryHashGetAsync failed: {Key}:{Field}", key, hashField);
                return RedisResult<TValue>.Fail(ex);
            }
        }

        public async Task<RedisResult<Dictionary<string, TValue>>> TryHashAllGetAsync<TValue>(string key)
        {
            try
            {
                if (!IsConnected)
                {
                    return RedisResult<Dictionary<string, TValue>>.Fail();
                }

                var entries = await mDatabase.HashGetAllAsync(key);
                var dict = new Dictionary<string, TValue>(entries.Length);

                foreach (var entry in entries)
                {
                    if (entry.Value.IsNullOrEmpty)
                    {
                        continue;
                    }

                    var value = JsonSerializer.Deserialize<TValue>(entry.Value!);
                    if (value is null)
                    {
                        continue;
                    }

                    dict[entry.Name!] = value;
                }

                return RedisResult<Dictionary<string, TValue>>.Ok(dict);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryHashAllGetAsync failed: {Key}", key);
                return RedisResult<Dictionary<string, TValue>>.Fail(ex);
            }
        }

        #endregion

        #region Key

        public async Task<RedisResult<bool>> TryKeyExistsAsync(string key)
        {
            try
            {
                if (!IsConnected)
                {
                    return RedisResult<bool>.Fail();
                }

                var exists = await mDatabase.KeyExistsAsync(key);
                return RedisResult<bool>.Ok(exists);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryKeyExistsAsync failed: {Key}", key);
                return RedisResult<bool>.Fail(ex);
            }
        }

        public async Task<RedisResult<bool>> TryKeyDeleteAsync(string key)
        {
            try
            {
                if (!IsConnected)
                {
                    return RedisResult<bool>.Fail();
                }

                var deleted = await mDatabase.KeyDeleteAsync(key);
                return RedisResult<bool>.Ok(deleted);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryKeyDeleteAsync failed: {Key}", key);
                return RedisResult<bool>.Fail(ex);
            }
        }

        public async Task<RedisResult<TimeSpan?>> TryGetExpiryRemainingAsync(string key)
        {
            try
            {
                if (!IsConnected)
                {
                    return RedisResult<TimeSpan?>.Fail();
                }

                var ttl = await mDatabase.KeyTimeToLiveAsync(key);
                return RedisResult<TimeSpan?>.Ok(ttl);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "TryGetExpiryRemainingAsync failed: {Key}", key);
                return RedisResult<TimeSpan?>.Fail(ex);
            }
        }

        public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry)
        {
            return await mDatabase.KeyExpireAsync(key, expiry);
        }

        #endregion

        public async Task<bool> PingAsync()
        {
            try
            {
                var latency = await mDatabase.PingAsync();
                return latency.TotalMilliseconds < 1000;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "PingAsync failed");
                return false;
            }
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
