using FluentAssertions;
using PetroineosChallenge.Core;

namespace PetroineosChallenge.Tests;


public class ExtractDateTimeProviderTests
{
    [Theory]
    [InlineData(1, "23:00")]
    [InlineData(2, "00:00")]
    [InlineData(4, "02:00")]
    [InlineData(24, "22:00")]
    public void Returns_LocalTimeString_When_Period_Is_Between_1_And_24(int period, string expected)
    {
        var result = ExtractDateTimeProvider.GetLocalTime(period);
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData(-50)]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(30)]
    public void ThrowsException_When_Period_Is_Not_Between_1_And_24(int period)
    {
        Action act = () => ExtractDateTimeProvider.GetLocalTime(period);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
    
}