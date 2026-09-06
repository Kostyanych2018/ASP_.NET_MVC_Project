using System.Globalization;

namespace GameStore.Application.Common;

public static class PriceParser
{
    public static bool TryParse(string? input, out decimal price)
    {
        price = 0m;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var normalized = input.Trim().Replace(',', '.');

        return decimal.TryParse(
            normalized,
            CultureInfo.InvariantCulture,
            out price);
    }
}
