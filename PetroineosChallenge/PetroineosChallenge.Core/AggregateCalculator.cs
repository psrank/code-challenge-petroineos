using Services;

namespace PetroineosChallenge.Core;

public class AggregateCalculator
{
    public static List<VolumeAggregation> AggregatePowerPeriods(IEnumerable<PowerTrade> records)
    {
        var result = records.SelectMany(x => x.Periods)
            .GroupBy(l => l.Period)
            .Select(cl => new VolumeAggregation
            (
                 ExtractDateTimeProvider.GetLocalTime(cl.Key),
                 cl.Sum(c => c.Volume)
            )).ToList();
        return result;
    }
}
