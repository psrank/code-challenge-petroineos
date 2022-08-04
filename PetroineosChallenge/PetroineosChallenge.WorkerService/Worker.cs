using Microsoft.Extensions.Options;
using PetroineosChallenge.Core;
using PetroineosChallenge.Core.Configuration;

namespace PetroineosChallenge.WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly CronScheduleProvider _cron;
    private readonly ITradeExtractor _priceExtractor;
    private  DateTime? _nextUtc;

    public Worker(ILogger<Worker> logger,
        CronScheduleProvider cron,
        ITradeExtractor priceExtractor)
    {
 
        _logger = logger;
        _cron = cron;
        _priceExtractor = priceExtractor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
            await _priceExtractor.RunExtractAsync(_nextUtc ?? DateTime.Now);
                
            var utcNow = DateTime.UtcNow;
            try
            {
                _nextUtc = _cron.GetNextOccurrence(utcNow);
            }
            catch (Exception ex)
            {
                _nextUtc = _cron.GetNearestMinute(utcNow);
                _logger.LogError(ex, "Problem with getting next cron schedule for {0}, falling back to run every minute", utcNow);
            }
            
                
            await Task.Delay(_nextUtc.Value - utcNow, stoppingToken);
        }
    }
}