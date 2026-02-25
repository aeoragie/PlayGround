namespace PlayGround.Shared.Extensions;

public static class ContainerExtensions
{
    private static readonly Random Random = new Random();

    public static TValue? GetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TKey : notnull
    {
        if (dict.Count == 0)
        {
            return default;
        }

        return dict.Values.ElementAt(Random.Next(dict.Count));
    }

    public static TKey? GetRandomKey<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TKey : notnull
    {
        if (dict.Count == 0)
        {
            return default;
        }

        return dict.Keys.ElementAt(Random.Next(dict.Count));
    }

    public static KeyValuePair<TKey, TValue>? GetRandom<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TKey : notnull
    {
        if (dict.Count == 0)
        {
            return default;
        }

        return dict.ElementAt(Random.Next(dict.Count));
    }
}
