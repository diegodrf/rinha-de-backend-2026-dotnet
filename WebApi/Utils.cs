using WebApi.DTOs;

namespace WebApi;

public static class Utils
{
    private const float Minimum = 0.0f;
    private const float Maximum = 1.0f;

    public static float Truncate(float value) => value switch
    {
        < Minimum => Minimum,
        > Maximum => Maximum,
        _ => value
    };
    
    public static readonly TransactionRequestDto RequestExample = new TransactionRequestDto()
    {
        Id = "tx-333099168",
        Transaction = new()
        {
            Amount = 9505.97f,
            Installments = 10,
            RequestedAt = new DateTime(2026, 3, 14, 5, 15, 12)
        },
        Customer = new()
        {
            AvgAmount = 81.28f,
            TxCount24H = 20,
            KnownMerchants = ["MERC-008", "MERC-007", "MERC-005"]
        },
        Merchant = new()
        {
            Id = "MERC-068",
            Mcc = "7802",
            AvgAmount = 54.86f
        },
        Terminal = new()
        {
            IsOnline = false,
            CardPresent = true,
            KmFromHome = 952.27f
        },
        LastTransaction = null
    };
}