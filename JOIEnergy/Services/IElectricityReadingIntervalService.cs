using JOIEnergy.Enums;

namespace JOIEnergy.Services
{
    public interface IElectricityReadingIntervalService
    {
        decimal GetCostUsage(string smartReadingId, Supplier supplier);
    }
}