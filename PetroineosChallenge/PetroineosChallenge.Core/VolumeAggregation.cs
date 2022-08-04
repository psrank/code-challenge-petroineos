namespace PetroineosChallenge.Core;

public class VolumeAggregation
{
    public VolumeAggregation(string localTime, double volume)
    {
        LocalTime = localTime;
        Volume = volume;
    }

    public string LocalTime { get; init; }

    public double Volume { get; init; }
}