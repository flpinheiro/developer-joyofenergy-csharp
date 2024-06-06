using System;
using System.Collections.Generic;
using System.Linq;
using JOIEnergy.Domain;

namespace JOIEnergy.Generator;

public static class ElectricityReadingGenerator
{
    public static List<ElectricityReading> Generate(int number)
    {
        var readings = new List<ElectricityReading>(number);
        var random = new Random();
        for (int i = 0; i < number; i++)
        {
            var reading = (decimal)random.NextDouble();
            var electricityReading = new ElectricityReading
            {
                Reading = reading,
                Time = DateTime.Now.AddSeconds(-i * 10)
            };
            readings.Add(electricityReading);
        }
        //readings.Sort((reading1, reading2) => reading1.Time.CompareTo(reading2.Time));
        readings.OrderBy(r => r.Time);
        return readings;
    }
}
