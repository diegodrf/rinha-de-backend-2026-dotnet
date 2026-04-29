using System.Diagnostics;
using System.Threading.Channels;

namespace WebApi.Services;

public class DataBackgroundService : BackgroundService
{
    private readonly IAntifraudService _antifraudService;
    private readonly Channel<TransactionChannelTemplate> _channel;
    private readonly ILogger<DataBackgroundService> _logger;

    public DataBackgroundService(
        Channel<TransactionChannelTemplate> channel, 
        IAntifraudService antifraudService, 
        ILogger<DataBackgroundService> logger)
    {
        _channel = channel;
        _antifraudService = antifraudService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var stopWatch = new Stopwatch();
        while (await _channel.Reader.WaitToReadAsync(stoppingToken))
        {
            if (!_channel.Reader.TryRead(out var item)) continue;
            
            stopWatch.Start();
                
            var response = await _antifraudService.GetScoreAsync(item.dto, stoppingToken);
                
            stopWatch.Stop();
                
            if (stopWatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning("Spent {time} milliseconds", stopWatch.ElapsedMilliseconds);    
            }
                
            stopWatch.Reset();

            item.responseChannel.Writer.TryWrite(response);
        }
    }
}