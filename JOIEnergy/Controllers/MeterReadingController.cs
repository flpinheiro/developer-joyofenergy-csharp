using JOIEnergy.Domain;
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
[Route("readings")]
public class MeterReadingController : Controller
{
    private readonly IMeterReadingService _meterReadingService;

    public MeterReadingController(IMeterReadingService meterReadingService)
    {
        _meterReadingService = meterReadingService;
    }

    /// <summary>
    /// save a new reading for a smart meter 
    /// </summary>
    /// <param name="meterReadings">smart meter parameters</param>
    /// <returns>empty object</returns>
    /// <remarks>
    /// example request:
    /// 
    ///     POST /readings/store
    ///     {
    ///         "smartMeterId": "smart-meter-0",
    ///         "electricityReadings": [
    ///             { 
    ///                 "time": "2024-06-04T19:45:59.0095147-03:00", 
    ///                 "reading": 0.880809498671685 
    ///             },
    ///             { 
    ///                 "time": "2024-06-04T19:50:59.0095147-03:00", 
    ///                 "reading": 0.720809498671685 
    ///             },
    ///         ]
    ///     }
    ///
    /// * reading is a decimal number between 0 and 1
    /// * time is a date time string
    /// 
    /// example of smart meters id 
    /// 
    /// * user    | smart meter id | provider
    /// * Sarah   | smart-meter-0  | Dr Evil's Dark Energy
    /// * Peter   | smart-meter-1  | The Green Eco
    /// * Charlie | smart-meter-2  | Dr Evil's Dark Energy
    /// * Andrea  | smart-meter-3  | Power for Everyone
    /// * Alex    | smart-meter-4  | The Green Eco
    /// </remarks>
    /// <response code="200">Returns the ok</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [HttpPost("store")]
    public ObjectResult Post([FromBody] MeterReadings meterReadings)
    {
        if (!IsMeterReadingsValid(meterReadings))
        {
            return new BadRequestObjectResult("Internal Server Error");
        }
        _meterReadingService.StoreReadings(meterReadings.SmartMeterId, meterReadings.ElectricityReadings);
        return new OkObjectResult("{}");
    }

    private static bool IsMeterReadingsValid(MeterReadings meterReadings)
    {
        string smartMeterId = meterReadings.SmartMeterId;
        List<ElectricityReading> electricityReadings = meterReadings.ElectricityReadings;
        return smartMeterId is not null && smartMeterId.Any()
                && electricityReadings is not null && electricityReadings.Any();
    }

    /// <summary>
    /// get the reading for a smart meter id
    /// </summary>
    /// <param name="smartMeterId">id of a smart meter</param>
    /// <returns>a list of eletric Reading <see cref="ElectricityReading"/></returns>
    /// <remarks>
    /// sample request: 
    /// 
    ///     GET /readings/read/smart-meter-0
    /// 
    /// example of smart meters id 
    /// * user    | smart meter id | provider
    /// * Sarah   | smart-meter-0  | Dr Evil's Dark Energy
    /// * Peter   | smart-meter-1  | The Green Eco
    /// * Charlie | smart-meter-2  | Dr Evil's Dark Energy
    /// * Andrea  | smart-meter-3  | Power for Everyone
    /// * Alex    | smart-meter-4  | The Green Eco
    /// </remarks>
    /// <response code="200">Returns the ok</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<ElectricityReading>))]
    [HttpGet("read/{smartMeterId}")]
    public ObjectResult GetReading([FromRoute] string smartMeterId)
    {
        return new OkObjectResult(_meterReadingService.GetReadings(smartMeterId));
    }
}
