using JOIEnergy.Domain;
using JOIEnergy.Enums;
using JOIEnergy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JOIEnergy.Controllers;

[ApiController]
[Produces("application/json")]
[Route("price-plans")]
public class PricePlanComparatorController : Controller
{
    private readonly IPricePlanService _pricePlanService;
    private readonly IAccountService _accountService;

    public PricePlanComparatorController(IPricePlanService pricePlanService, IAccountService accountService)
    {
        this._pricePlanService = pricePlanService;
        this._accountService = accountService;
    }

    /// <summary>
    /// View Current Price Plan and Compare Usage Cost Against all Price Plans
    /// </summary>
    /// <param name="smartMeterId">One of the smart meters' id listed above</param>
    /// <returns></returns>
    /// <remarks>
    /// sample request 
    /// 
    ///     GET /price-plans/compare-all/smart-meter-0
    ///     
    /// list of smart meters id 
    /// * user    | smart meter id | provider
    /// * Sarah   | smart-meter-0  | Dr Evil's Dark Energy
    /// * Peter   | smart-meter-1  | The Green Eco
    /// * Charlie | smart-meter-2  | Dr Evil's Dark Energy
    /// * Andrea  | smart-meter-3  | Power for Everyone
    /// * Alex    | smart-meter-4  | The Green Eco
    /// </remarks>
    /// <response code="200">Returns the ok</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Dictionary<string, decimal>))]
    [HttpGet("compare-all/{smartMeterId}")]
    public ObjectResult CalculatedCostForEachPricePlan([FromRoute]string smartMeterId)
    {
        Supplier pricePlanId = _accountService.GetPricePlanIdForSmartMeterId(smartMeterId);
        Dictionary<string, decimal> costPerPricePlan = _pricePlanService.GetConsumptionCostOfElectricityReadingsForEachPricePlan(smartMeterId);
        //if (!costPerPricePlan.Any())
        //{
        //    return new NotFoundObjectResult(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
        //}

        return
            costPerPricePlan.Any() ? 
            new ObjectResult(costPerPricePlan) : 
            new NotFoundObjectResult(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
    }

    /// <summary>
    /// View Recommended Price Plans for Usage
    /// </summary>
    /// <param name="smartMeterId">One of the smart meters' id listed above</param>
    /// <param name="limit">(Optional) limit the number of plans to be displayed</param>
    /// <returns>
    /// <para>
    ///     Exemple of output
    /// </para>
    /// <code>
    /// [
    ///   {
    ///     "key": "PowerForEveryone",
    ///     "value": 9.487181867550794
    ///   },
    ///   {
    ///     "key": "TheGreenEco",
    ///     "value": 18.974363735101587
    ///   }
    /// ]
    /// </code>
    /// </returns>
    /// <remarks>
    /// sample request 
    /// 
    ///     GET /price-plans/recommend/smart-meter-0
    ///     
    ///     GET /price-plans/recommend/smart-meter-0?limit=2
    /// 
    /// Exemple of output
    /// <code>
    /// [
    ///   {
    ///     "key": "PowerForEveryone",
    ///     "value": 9.487181867550794
    ///   },
    ///   {
    ///     "key": "TheGreenEco",
    ///     "value": 18.974363735101587
    ///   }
    /// ]
    /// </code>
    /// list of smart meters id 
    /// * user    | smart meter id | provider
    /// * Sarah   | smart-meter-0  | Dr Evil's Dark Energy
    /// * Peter   | smart-meter-1  | The Green Eco
    /// * Charlie | smart-meter-2  | Dr Evil's Dark Energy
    /// * Andrea  | smart-meter-3  | Power for Everyone
    /// * Alex    | smart-meter-4  | The Green Eco
    /// </remarks>
    /// <response code="200">Returns the ok</response>
    /// <response code="404">Returns the not found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Dictionary<string, decimal>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("recommend/{smartMeterId}")]
    public ObjectResult RecommendCheapestPricePlans([FromRoute] string smartMeterId,[FromQuery] int? limit = null) {
        var consumptionForPricePlans = _pricePlanService.GetConsumptionCostOfElectricityReadingsForEachPricePlan(smartMeterId);

        if (!consumptionForPricePlans.Any()) {
            return new NotFoundObjectResult(string.Format("Smart Meter ID ({0}) not found", smartMeterId));
        }

        var recommendations = consumptionForPricePlans.OrderBy(pricePlanComparison => pricePlanComparison.Value);

        if (limit is not null && limit < recommendations.Count())
        {
            return new ObjectResult(recommendations.Take(limit.Value));
        }

        return new ObjectResult(recommendations);
    }
}
