using JOIEnergy.Services;
using Microsoft.AspNetCore.Mvc;

namespace JOIEnergy.Controllers;

[Controller]
public class EletricCostController(IAccountService accountService, IElectricityReadingIntervalService readingIntervalService) : Controller
{
    [HttpGet("{smartMeterId}")]
    public ActionResult GetCost([FromRoute] string smartMeterId)
    {
        var supplier = accountService.GetPricePlanIdForSmartMeterId(smartMeterId);
        if (supplier is Enums.Supplier.NullSupplier)
        {
            return BadRequest($"the smart meter id: {smartMeterId} doen't have a supplier");
        }
         var cost = readingIntervalService.GetCostUsage(smartMeterId, supplier);



        return Ok($"the total cost was: {cost}");
    }

    
}
