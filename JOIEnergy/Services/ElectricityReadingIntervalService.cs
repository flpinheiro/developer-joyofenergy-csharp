using JOIEnergy.Domain;
using JOIEnergy.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace JOIEnergy.Services;

public class ElectricityReadingIntervalService(IMeterReadingService meterReadingService, List<PricePlan> plans) : IElectricityReadingIntervalService
{
    public decimal GetCostUsage(string smartReadingId, Supplier supplier)
    {

        var readingInterval = ElectricityReadingInterval.getPreviousWeekInterval();

        var reading = meterReadingService.GetReadings(smartReadingId)
            //.Where(x => x.Time >= readingInterval.Start && x.Time <= readingInterval.End)
            .ToList();

        if (reading == null || !reading.Any()) return 0m;

        var averageReadings = PricePlanService.CalculateAverageReading(reading);

        var plan = plans.FirstOrDefault(x => x.EnergySupplier.Equals(supplier));

        var cost = averageReadings * plan.UnitRate;

        return cost;
    }


}
