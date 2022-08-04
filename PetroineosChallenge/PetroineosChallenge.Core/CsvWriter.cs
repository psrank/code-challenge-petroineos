using System.Globalization;
using Microsoft.Extensions.Logging;

namespace PetroineosChallenge.Core;


public class CsvWriter
{
    private readonly ILogger<CsvWriter> _logger;

    public CsvWriter(ILogger<CsvWriter> logger)
    {
        _logger = logger;
    }
    
    public async Task WriteToFile(string basePath, List<VolumeAggregation> result)
    {
        EnsurePathExists(basePath);
        
        var fileName = $"PowerPosition_{DateTime.Now.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture)}";
        var fullFilePath = Path.Combine(basePath, $"{fileName}.csv");

        _logger.LogInformation("Writing to file {0}", fullFilePath);
        
        await using (var writer = new StreamWriter(fullFilePath))
        {
            await writer.WriteLineAsync("Local Time,Volume");
            foreach (var powerPeriod in result)
            {
                await writer.WriteLineAsync($"{powerPeriod.LocalTime},{powerPeriod.Volume}");
            }

            await writer.FlushAsync();
        }
    }
    
    private void EnsurePathExists(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unable to create directory for path {0} - Unauthorized Access ", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to create directory for extract");
        }
    }
}