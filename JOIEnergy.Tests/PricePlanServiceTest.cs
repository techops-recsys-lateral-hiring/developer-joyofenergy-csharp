using System;
using JOIEnergy.Services;
using Xunit;
using Moq;
using JOIEnergy.Domain;
using System.Collections.Generic;

namespace JOIEnergy.Tests
{
    public class PricePlanServiceTest
    {
        private PricePlanService _pricePlanService;
        private readonly Mock<MeterReadingService> _mockMeterReadingService;
        private List<PricePlan> _pricePlans;

        public PricePlanServiceTest()
        {
            _mockMeterReadingService = new Mock<MeterReadingService>();
            _pricePlanService = new PricePlanService(_pricePlans, _mockMeterReadingService.Object);

            _mockMeterReadingService.Setup(service => service.GetReadings(It.IsAny<string>())).Returns(new List<ElectricityReading>(){new ElectricityReading(),
                new ElectricityReading()});
        }
    }
}
