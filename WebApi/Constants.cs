using System.Collections.Frozen;

namespace WebApi;

public static class Constants
{
    public const decimal MaxAmount = 10000;
    public const decimal MaxInstallments = 12;
    public const decimal AmountVsAvgRatio = 10;
    public const decimal MaxMinutes = 1440;
    public const decimal MaxKm = 1000;
    public const decimal MaxTxCount24H = 20;
    public const decimal MaxMerchantAvgAmount = 10000;

    public static readonly FrozenDictionary<string, decimal> MccRisk = new Dictionary<string, decimal>
    {
        { "5411", 0.15m },
        { "5812", 0.30m },
        { "5912", 0.20m },
        { "5944", 0.45m },
        { "7801", 0.80m },
        { "7802", 0.75m },
        { "7995", 0.85m },
        { "4511", 0.35m },
        { "5311", 0.25m },
        { "5999", 0.50m }
    }.ToFrozenDictionary();
}
