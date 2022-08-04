using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetroineosChallenge.Core.Configuration;
using Polly;
using Services;

namespace PetroineosChallenge.Core;


public interface ITradeExtractor
{
    Task RunExtractAsync(DateTime extractDateTime);
}

public class TradeExtractor : ITradeExtractor
{
    private readonly ILogger<TradeExtractor> _logger;
    private readonly IPowerService _powerService;
    private readonly IOptions<ExtractSettings> _settings;
    private readonly CsvWriter _csvWriter;

    public TradeExtractor(ILogger<TradeExtractor> logger,
        IPowerService powerService,
        IOptions<ExtractSettings> settings,
        CsvWriter csvWriter)
    {
        _ = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger;
        _powerService = powerService;
        _settings = settings;
        _csvWriter = csvWriter;
    }
    
    public async Task RunExtractAsync(DateTime extractDateTime)
    {
        _logger.LogDebug("--> Task on thread {0} started. Run date {1}",
            Thread.CurrentThread.ManagedThreadId, extractDateTime.ToLongTimeString());
        
        var maxRetries = 5;
        var retryPolicy = Policy.Handle<PowerServiceException>()
            .WaitAndRetryAsync(retryCount: maxRetries, sleepDurationProvider: (attemptCount) => TimeSpan.FromSeconds(attemptCount * 10),
                onRetry: (exception, sleepDuration, attemptNumber, _) =>
                {
                    _logger.LogError(exception, $"Power service error. Unable to get trades. \n Retrying in {sleepDuration}. {attemptNumber} / {maxRetries}");
                });

        var records = await retryPolicy.ExecuteAsync(() => _powerService.GetTradesAsync(extractDateTime));
        
        var aggregatedRecords = AggregateCalculator.AggregatePowerPeriods(records);

        var basePath = string.IsNullOrWhiteSpace(_settings.Value.ExtractDestinationPath) ? 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Extract") : _settings.Value.ExtractDestinationPath;
        await _csvWriter.WriteToFile(basePath, aggregatedRecords);
        
        
        await Task.Delay(4000);
        _logger.LogDebug("--->! Task on thread {0} finished. Run date {1}",
            Thread.CurrentThread.ManagedThreadId, extractDateTime.ToLongTimeString());
    }
}