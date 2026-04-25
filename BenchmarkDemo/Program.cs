using System.Buffers;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using WebApi.Dtos;
using WebApi.Services;

BenchmarkRunner.Run<MyBenchmark>();

[MemoryDiagnoser]
public class MyBenchmark
{
    private readonly TransactionRequestDto _dto = JsonSerializer.Deserialize<TransactionRequestDto>("""
                                                                    {
                                                                    "id": "tx-3330991687",
                                                                    "transaction":      { "amount": 9505.97, "installments": 10, "requested_at": "2026-03-14T05:15:12Z" },
                                                                    "customer":         { "avg_amount": 81.28, "tx_count_24h": 20, "known_merchants": ["MERC-008", "MERC-007", "MERC-005"] },
                                                                    "merchant":         { "id": "MERC-068", "mcc": "7802", "avg_amount": 54.86 },
                                                                    "terminal":         { "is_online": false, "card_present": true, "km_from_home": 952.27 },
                                                                    "last_transaction": null
                                                                    }
                                                                    """);
    
    [Benchmark]
    public decimal[] TestPerformance()
    {
        var pool = ArrayPool<decimal>.Shared;
        var r = pool.Rent(14);
        var array = EmbeddingService.Embedding(_dto, r);
        pool.Return(r, true);
        return array;
    }
}