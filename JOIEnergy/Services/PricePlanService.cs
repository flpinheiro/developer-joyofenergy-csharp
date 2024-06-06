using System;
using System.Collections.Generic;
using System.Linq;
using JOIEnergy.Domain;

namespace JOIEnergy.Services;

public class PricePlanService : IPricePlanService
{
    public interface Debug { void Log(string s); };

    private readonly List<PricePlan> _pricePlans;
    private IMeterReadingService _meterReadingService;

    public PricePlanService(List<PricePlan> pricePlan, IMeterReadingService meterReadingService)
    {
        _pricePlans = pricePlan;
        _meterReadingService = meterReadingService;
    }

    private static decimal CalculateAverageReading(List<ElectricityReading> electricityReadings)
    {
        var newSummedReadings = electricityReadings.Select(readings => readings.Reading).Aggregate(0m,(reading, accumulator) => reading + accumulator);

        return newSummedReadings / electricityReadings.Count();
    }
    // =>
    // electricityReadings.Select(readings => readings.Reading).Aggregate((reading, accumulator) => reading + accumulator) / electricityReadings.Count();
    // electricityReadings.Select(readings => readings.Reading).Sum() / electricityReadings.Count();

    private static decimal CalculateTimeElapsed(List<ElectricityReading> electricityReadings)
    {
        var first = electricityReadings.Min(reading => reading.Time);
        var last = electricityReadings.Max(reading => reading.Time);
        
        return (decimal)(last - first).TotalHours;
    }
    private static decimal CalculateCost(List<ElectricityReading> electricityReadings, PricePlan pricePlan)
    {
        var average = CalculateAverageReading(electricityReadings);
        var timeElapsed = CalculateTimeElapsed(electricityReadings);
        var averagedCost = average/timeElapsed;
        return averagedCost * pricePlan.UnitRate;
    }

    public Dictionary<string, decimal> GetConsumptionCostOfElectricityReadingsForEachPricePlan(String smartMeterId)
    {
        List<ElectricityReading> electricityReadings = _meterReadingService.GetReadings(smartMeterId);

        if (!electricityReadings.Any())
        {
            return new Dictionary<string, decimal>();
        }
        return _pricePlans.ToDictionary(plan => plan.EnergySupplier.ToString(), plan => CalculateCost(electricityReadings, plan));
    }
}
