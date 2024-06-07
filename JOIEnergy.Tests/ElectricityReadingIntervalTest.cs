using JOIEnergy.Domain;
using System;
using Xunit;

namespace JOIEnergy.Tests;

public class ElectricityReadingIntervalTest
{
    public ElectricityReadingIntervalTest()
    {
    }

    [Fact]
    public void GetLastWeekFromToday()
    {
        Assert.Equal(new DateTime(2021, 9, 26), ElectricityReadingInterval.getPreviousWeekInterval(new DateTime(2021, 10, 5, 12, 00, 00)).Start);
        Assert.Equal(new DateTime(2021, 10, 3), ElectricityReadingInterval.getPreviousWeekInterval(new DateTime(2021, 10, 5, 12, 00, 00)).End);
    }
}
