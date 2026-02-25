using System.Globalization;

namespace PlayGround.Shared.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }

    public static bool TryParseEnum<TEnum>(this string input, out TEnum result) where TEnum : struct, Enum
    {
        result = default;
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        return Enum.TryParse<TEnum>(input, true, out result);
    }

    public static TEnum ParseEnum<TEnum>(this string input) where TEnum : struct, Enum
    {
        if (TryParseEnum<TEnum>(input, out var result) == false)
        {
            throw new ArgumentException($"Input string cannot be converted to the enum type '{typeof(TEnum).Name}' because it is empty.", nameof(input));
        }

        return result;
    }
}
