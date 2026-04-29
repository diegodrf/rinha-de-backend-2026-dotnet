using System.Buffers;
using System.Collections.Concurrent;
using System.Threading.Channels;
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

public static class EmbeddingPool
{
    private const int FixedLength = 14;
    private static readonly ConcurrentQueue<float[]> _queue = new();
    
    public static float[] Rent()
    {
        return _queue.TryDequeue(out var value) ? value : new float[FixedLength];
    }

    public static void Return(float[] array)
    {
        _queue.Enqueue(array);
    }
}

public static class ChannelPool
{
    private static readonly ConcurrentQueue<Channel<TransactionResponseDto>> _queue = new();
    
    public static Channel<TransactionResponseDto> Rent()
    {
        if (_queue.TryDequeue(out var value)) return value;

        return Channel.CreateBounded<TransactionResponseDto>(1);
    }

    public static void Return(Channel<TransactionResponseDto> channel)
    {
        _queue.Enqueue(channel);
    }
}