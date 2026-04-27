using System.Collections.Frozen;

namespace WebApi;

public static class Constants
{
    public const int DatabaseTargetResponseTimeInMilliseconds = 300;
    
    public const float MaxAmount = 10000;
    public const float MaxInstallments = 12;
    public const float AmountVsAvgRatio = 10;
    public const float MaxMinutes = 1440;
    public const float MaxKm = 1000;
    public const float MaxTxCount24H = 20;
    public const float MaxMerchantAvgAmount = 10000;

    public static readonly FrozenDictionary<string, float> MccRisk = new Dictionary<string, float>
    {
        { "5411", 0.15f },
        { "5812", 0.30f },
        { "5912", 0.20f },
        { "5944", 0.45f },
        { "7801", 0.80f },
        { "7802", 0.75f },
        { "7995", 0.85f },
        { "4511", 0.35f },
        { "5311", 0.25f },
        { "5999", 0.50f }
    }.ToFrozenDictionary();
}
