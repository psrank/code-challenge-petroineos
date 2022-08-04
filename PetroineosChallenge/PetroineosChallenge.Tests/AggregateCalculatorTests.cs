using FluentAssertions;
using PetroineosChallenge.Core;
using Services;

namespace PetroineosChallenge.Tests;

public class AggregateCalculatorTests
{
    [Fact]
    public void Sums_Volumes_Of_Multiple_PowerPeriods()
    {
        // arrange
        var powerTrade1 = PowerTrade.Create(DateTime.Today, 2);
        powerTrade1.Periods[0] = new PowerPeriod {Period = 1, Volume = 100};
        powerTrade1.Periods[1] = new PowerPeriod {Period = 2, Volume = 100};
        
        var powerTrade2 = PowerTrade.Create(DateTime.Today, 2);
        powerTrade2.Periods[0] = new PowerPeriod {Period = 1, Volume = 50};
        powerTrade2.Periods[1] = new PowerPeriod {Period = 2, Volume = -20};

        var input = new List<PowerTrade>
        {
            powerTrade1,
            powerTrade2
        };
        
        var expectedResult = new List<VolumeAggregation>
        {
            new VolumeAggregation ("23:00", 150),
            new VolumeAggregation ("00:00", 80)
        };

        // act
        var result = AggregateCalculator.AggregatePowerPeriods(input);

        // assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    
}