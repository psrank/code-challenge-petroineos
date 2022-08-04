using Cronos;
using Microsoft.Extensions.Options;
using PetroineosChallenge.Core.Configuration;

namespace PetroineosChallenge.Core;

public class CronScheduleProvider
{
    private readonly CronExpression _cron;
    public CronScheduleProvider(IOptions<ExtractSettings> settings)
    {
        _ = settings ?? throw new ArgumentNullException(nameof(settings));
        var schedule = GetCronSchedule(settings.Value.ScheduledIntervalInMinutes);
        _cron = CronExpression.Parse(schedule);
    }
    private string GetCronSchedule(int minutes)
    {
        if (minutes < 1 || minutes > 60) throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes for cron schedule should be between 1 and 60");

        return $"*/{minutes} * * * *";
    }

    public DateTime GetNextOccurrence(DateTime datetime)
    {
        var nextRunDate = _cron.GetNextOccurrence(datetime);
        if (!nextRunDate.HasValue) throw new InvalidOperationException("Problem with getting next cron schedule");

        return nextRunDate.Value;
    }

    public DateTime GetNearestMinute(DateTime datetime)
    {
        var d =TimeSpan.FromMinutes(1);
        return new DateTime((datetime.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, datetime.Kind);
    }
    
}