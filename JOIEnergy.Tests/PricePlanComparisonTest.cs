using JOIEnergy.Controllers;
using JOIEnergy.Domain;
using JOIEnergy.Enums;
using JOIEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json.Linq;

namespace JOIEnergy.Tests
{
    public class PricePlanComparisonTest
    {
        private MeterReadingService meterReadingService;
        private PricePlanComparatorController controller;
        private Dictionary<string, Supplier> smartMeterToPricePlanAccounts = new Dictionary<string, Supplier>();
        private static String SMART_METER_ID = "smart-meter-id";

        public PricePlanComparisonTest()
        {
            var readings = new Dictionary<string, List<Domain.ElectricityReading>>();
            meterReadingService = new MeterReadingService(readings);
            var pricePlans = new List<PricePlan>() { 
                new PricePlan() { EnergySupplier = Supplier.DrEvilsDarkEnergy, UnitRate = 10, PeakTimeMultiplier = NoMultipliers() }, 
                new PricePlan() { EnergySupplier = Supplier.TheGreenEco, UnitRate = 2, PeakTimeMultiplier = NoMultipliers() },
                new PricePlan() { EnergySupplier = Supplier.PowerForEveryone, UnitRate = 1, PeakTimeMultiplier = NoMultipliers() } 
            };
            var pricePlanService = new PricePlanService(pricePlans, meterReadingService);
            var accountService = new AccountService(smartMeterToPricePlanAccounts);
            controller = new PricePlanComparatorController(pricePlanService, accountService);
        }

        [Fact]
        public void ShouldCalculateCostForMeterReadingsForEveryPricePlan()
        {
            var electricityReading = new ElectricityReading() { Time = DateTime.Now.AddHours(-1), Reading = 15.0m };
            var otherReading = new ElectricityReading() { Time = DateTime.Now, Reading = 5.0m };
            meterReadingService.StoreReadings(SMART_METER_ID, new List<ElectricityReading>() { electricityReading, otherReading });

            var result = controller.CalculatedCostForEachPricePlan(SMART_METER_ID).Value;

            var actualCosts = ((JObject)result).ToObject<Dictionary<string, decimal>>();
            Assert.Equal(3, actualCosts.Count);
            Assert.Equal(100m, actualCosts["" + Supplier.DrEvilsDarkEnergy], 3);
            Assert.Equal(20m, actualCosts["" + Supplier.TheGreenEco], 3);
            Assert.Equal(10m, actualCosts["" + Supplier.PowerForEveryone], 3);
        }

        [Fact]
        public void ShouldRecommendCheapestPricePlansNoLimitForMeterUsage()
        {
            meterReadingService.StoreReadings(SMART_METER_ID, new List<ElectricityReading>() {
                new ElectricityReading() { Time = DateTime.Now.AddMinutes(-30), Reading = 35m },
                new ElectricityReading() { Time = DateTime.Now, Reading = 3m }
            });

            object result = controller.RecommendCheapestPricePlans(SMART_METER_ID, null).Value;
            var recommendations = ((IEnumerable<KeyValuePair<string, decimal>>)result).ToList();

            Assert.Equal("" + Supplier.PowerForEveryone, recommendations[0].Key);
            Assert.Equal("" + Supplier.TheGreenEco, recommendations[1].Key);
            Assert.Equal("" + Supplier.DrEvilsDarkEnergy, recommendations[2].Key);
            Assert.Equal(38m, recommendations[0].Value, 3);
            Assert.Equal(76m, recommendations[1].Value, 3);
            Assert.Equal(380m, recommendations[2].Value, 3);
            Assert.Equal(3, recommendations.Count);
        }

        [Fact]
        public void ShouldRecommendLimitedCheapestPricePlansForMeterUsage() 
        {
            meterReadingService.StoreReadings(SMART_METER_ID, new List<ElectricityReading>() {
                new ElectricityReading() { Time = DateTime.Now.AddMinutes(-45), Reading = 5m },
                new ElectricityReading() { Time = DateTime.Now, Reading = 20m }
            });

            object result = controller.RecommendCheapestPricePlans(SMART_METER_ID, 2).Value;
            var recommendations = ((IEnumerable<KeyValuePair<string, decimal>>)result).ToList();

            Assert.Equal("" + Supplier.PowerForEveryone, recommendations[0].Key);
            Assert.Equal("" + Supplier.TheGreenEco, recommendations[1].Key);
            Assert.Equal(16.667m, recommendations[0].Value, 3);
            Assert.Equal(33.333m, recommendations[1].Value, 3);
            Assert.Equal(2, recommendations.Count);
        }

        [Fact]
        public void ShouldRecommendCheapestPricePlansMoreThanLimitAvailableForMeterUsage()
        {
            meterReadingService.StoreReadings(SMART_METER_ID, new List<ElectricityReading>() {
                new ElectricityReading() { Time = DateTime.Now.AddMinutes(-30), Reading = 35m },
                new ElectricityReading() { Time = DateTime.Now, Reading = 3m }
            });

            object result = controller.RecommendCheapestPricePlans(SMART_METER_ID, 5).Value;
            var recommendations = ((IEnumerable<KeyValuePair<string, decimal>>)result).ToList();

            Assert.Equal(3, recommendations.Count);
        }

        [Fact]
        public void GivenNoMatchingMeterIdShouldReturnNotFound()
        {
            Assert.Equal(404, controller.CalculatedCostForEachPricePlan("not-found").StatusCode);
        }

        private static List<PeakTimeMultiplier> NoMultipliers()
        {
            return new List<PeakTimeMultiplier>();
        }
    }
}
