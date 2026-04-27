using System.Text.Json.Serialization;

namespace WebApi.DTOs;

public record TransactionRequestDto
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;

    [JsonPropertyName("transaction")] public Transaction Transaction { get; init; } = new();

    [JsonPropertyName("customer")] public Customer Customer { get; init; } = new();

    [JsonPropertyName("merchant")] public Merchant Merchant { get; init; } = new();

    [JsonPropertyName("terminal")] public Terminal Terminal { get; init; } = new();

    [JsonPropertyName("last_transaction")] public LastTransaction? LastTransaction { get; init; }
}

public record Terminal
{
    [JsonPropertyName("is_online")] public bool IsOnline { get; init; }

    [JsonPropertyName("card_present")] public bool CardPresent { get; init; }

    [JsonPropertyName("km_from_home")] public float KmFromHome { get; init; }
}

public record Merchant
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;

    [JsonPropertyName("mcc")] public string Mcc { get; init; } = string.Empty;

    [JsonPropertyName("avg_amount")] public float AvgAmount { get; init; }
}

public record Transaction
{
    [JsonPropertyName("amount")] public float Amount { get; init; }
    [JsonPropertyName("installments")] public int Installments { get; init; }
    [JsonPropertyName("requested_at")] public DateTime RequestedAt { get; init; }
}

public record Customer
{
    [JsonPropertyName("avg_amount")] public float AvgAmount { get; init; }

    [JsonPropertyName("tx_count_24h")] public int TxCount24H { get; init; }

    [JsonPropertyName("known_merchants")] public string[] KnownMerchants { get; init; } = [];
}

public record LastTransaction
{
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; init; }

    [JsonPropertyName("km_from_current")] public float KmFromCurrent { get; init; }
}