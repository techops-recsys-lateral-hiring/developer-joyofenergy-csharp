using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JOIEnergy.Domain;
using JOIEnergy.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JOIEnergy.Controllers
{
    [Route("readings")]
    public class MeterReadingController(IMeterReadingService meterReadingService) : Controller
    {
        // POST api/values
        [HttpPost ("store")]
        public ObjectResult Post([FromBody]MeterReadings meterReadings)
        {
            if (!IsMeterReadingsValid(meterReadings)) {
                return new BadRequestObjectResult("Internal Server Error");
            }
            meterReadingService.StoreReadings(meterReadings.SmartMeterId,meterReadings.ElectricityReadings);
            return new OkObjectResult("{}");
        }

        private bool IsMeterReadingsValid(MeterReadings meterReadings)
        {
            String smartMeterId = meterReadings.SmartMeterId;
            List<ElectricityReading> electricityReadings = meterReadings.ElectricityReadings;
            return smartMeterId != null && smartMeterId.Any()
                    && electricityReadings != null && electricityReadings.Any();
        }

        [HttpGet("read/{smartMeterId}")]
        public ObjectResult GetReading(string smartMeterId) {
            return new OkObjectResult(meterReadingService.GetReadings(smartMeterId));
        }
    }
}
