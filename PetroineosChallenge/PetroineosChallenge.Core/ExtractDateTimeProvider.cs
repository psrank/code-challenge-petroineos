namespace PetroineosChallenge.Core;

public class ExtractDateTimeProvider
{
    public static string GetLocalTime(int period)
    {
        if (period < 1 || period > 24) throw new ArgumentOutOfRangeException(nameof(period), "Period should be between 1 and 24");

        int periodIndexAdjustment = -1;
        int londonTimeDifference = -1;
        
        return new TimeOnly(period + periodIndexAdjustment , 00, 00).AddHours(londonTimeDifference).ToString("HH:mm");
    }
}