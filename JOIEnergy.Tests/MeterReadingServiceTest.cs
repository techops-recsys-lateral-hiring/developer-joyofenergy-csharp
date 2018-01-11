using System;
using System.Collections.Generic;
using JOIEnergy.Services;
using JOIEnergy.Domain;
using Xunit;

namespace JOIEnergy.Tests
{
    public class MeterReadingServiceTest
    {
        private MeterReadingService meterReadingService;

        public MeterReadingServiceTest()
        {
            meterReadingService = new MeterReadingService(new Dictionary<string, List<ElectricityReading>>());
        }

        [Fact]
        public void GivenMeterIdThatDoesNotExistShouldReturnNull() {
            Assert.Empty(meterReadingService.GetReadings("unknown-id"));
        }

        [Fact]
        public void GivenMeterReadingThatExistsShouldReturnMeterReadings()
        {
            meterReadingService.StoreReadings("random-id", new List<ElectricityReading>());
            Assert.Equal(new List<ElectricityReading>(), meterReadingService.GetReadings("random-id"));
        }
    }
}
