using System.Text.Json.Serialization;

namespace WebApi.Dtos;

public record struct TransactionRequestDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    
    [JsonPropertyName("transaction")]
    public Transaction Transaction { get; init; }
    
    [JsonPropertyName("customer")]
    public Customer Customer { get; init; }
    
    [JsonPropertyName("merchant")]
    public Merchant Merchant { get; init; }
    
    [JsonPropertyName("terminal")]
    public Terminal Terminal { get; init; }
    
    [JsonPropertyName("last_transaction")]
    public LastTransaction? LastTransaction { get; init; }
}

public record struct Terminal
{
    [JsonPropertyName("is_online")]
    public bool IsOnline { get; init; }
    
    [JsonPropertyName("card_present")]
    public bool CardPresent { get; init; }
    
    [JsonPropertyName("km_from_home")]
    public decimal KmFromHome { get; init; }
}

public record struct Merchant
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    
    [JsonPropertyName("mcc")]
    public string Mcc { get; init; }
    
    [JsonPropertyName("avg_amount")]
    public decimal AvgAmount { get; init; }
}

public record struct Transaction
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
    [JsonPropertyName("installments")]
    public int Installments { get; init; }
    [JsonPropertyName("requested_at")]
    public DateTime RequestedAt { get; init; }
}

public record struct Customer
{
    [JsonPropertyName("avg_amount")]
    public decimal AvgAmount { get; init; }
    
    [JsonPropertyName("tx_count_24h")]
    public int TxCount24H { get; init; }
    
    [JsonPropertyName("known_merchants")]
    public HashSet<string> KnownMerchants { get; init; }
}

public record struct LastTransaction
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    
    [JsonPropertyName("km_from_current")]
    public decimal KmFromCurrent { get; set; }
}
