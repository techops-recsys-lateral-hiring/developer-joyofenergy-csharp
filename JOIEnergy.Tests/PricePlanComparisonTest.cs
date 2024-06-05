using JOIEnergy.Controllers;
using JOIEnergy.Domain;
using JOIEnergy.Enums;
using JOIEnergy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace JOIEnergy.Tests
{
    public class PricePlanComparisonTest
    {
        private MeterReadingService meterReadingService;
        private PricePlanComparatorController controller;

        private static string PRICE_PLAN_1_ID = "test-supplier";
        private static string PRICE_PLAN_2_ID = "best-supplier";
        private static string PRICE_PLAN_3_ID = "second-best-supplier";
        private static string SMART_METER_ID = "smart-meter-id";

        public PricePlanComparisonTest()
        {
            var readings = new Dictionary<string, List<Domain.ElectricityReading>>();
            meterReadingService = new MeterReadingService(readings);
            var pricePlans = new List<PricePlan>() { 
                new PricePlan() { PlanName = PRICE_PLAN_1_ID, UnitRate = 10, PeakTimeMultiplier = NoMultipliers() }, 
                new PricePlan() { PlanName = PRICE_PLAN_2_ID, UnitRate = 1, PeakTimeMultiplier = NoMultipliers() },
                new PricePlan() { PlanName = PRICE_PLAN_3_ID, UnitRate = 2, PeakTimeMultiplier = NoMultipliers() } 
            };
            var pricePlanService = new PricePlanService(pricePlans, meterReadingService);
            var smartMeterToPricePlanAccounts = new Dictionary<string, string>
            {
                { SMART_METER_ID, PRICE_PLAN_1_ID }
            };
            var accountService = new AccountService(smartMeterToPricePlanAccounts);
            controller = new PricePlanComparatorController(pricePlanService, accountService);
        }

        [Fact]
        public void ShouldCalculateCostForMeterReadingsForEveryPricePlan()
        {
            var electricityReading = new ElectricityReading() { Time = DateTime.Now.AddHours(-1), Reading = 15.0m };
            var otherReading = new ElectricityReading() { Time = DateTime.Now, Reading = 5.0m };
            meterReadingService.StoreReadings(SMART_METER_ID, new List<ElectricityReading>() { electricityReading, otherReading });

            Dictionary<string, object> result = controller.CalculatedCostForEachPricePlan(SMART_METER_ID).Value as Dictionary<string, object>;

            Assert.NotNull(result);
            Assert.Equal(PRICE_PLAN_1_ID, result[PricePlanComparatorController.PRICE_PLAN_ID_KEY]);
            var expected = new Dictionary<string, decimal>() {
                { PRICE_PLAN_1_ID, 100m },
                { PRICE_PLAN_2_ID, 10m },
                { PRICE_PLAN_3_ID, 20m },
            };
            Assert.Equal(expected, result[PricePlanComparatorController.PRICE_PLAN_COMPARISONS_KEY]);
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
            var expected = new List<KeyValuePair<string, decimal>>() {
                new(PRICE_PLAN_2_ID, 38m),
                new(PRICE_PLAN_3_ID, 76m),
                new(PRICE_PLAN_1_ID, 380m),
            };
            Assert.Equal(expected, recommendations);
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
            var expected = new List<KeyValuePair<string, decimal>>() {
                new(PRICE_PLAN_2_ID, 16.667m),
                new(PRICE_PLAN_3_ID, 33.333m),
            };
            Assert.Equal(expected, recommendations);
        }

        [Fact]
        public void ShouldRecommendCheapestPricePlansMoreThanLimitAvailableForMeterUsage()
        {
            meterReadingService.StoreReadings(SMART_METER_ID, new List<ElectricityReading>() {
                new ElectricityReading() { Time = DateTime.Now.AddMinutes(-60), Reading = 25m },
                new ElectricityReading() { Time = DateTime.Now, Reading = 3m }
            });

            object result = controller.RecommendCheapestPricePlans(SMART_METER_ID, 5).Value;

            var recommendations = ((IEnumerable<KeyValuePair<string, decimal>>)result).ToList();
            var expected = new List<KeyValuePair<string, decimal>>() {
                new(PRICE_PLAN_2_ID, 14m),
                new(PRICE_PLAN_3_ID, 28m),
                new(PRICE_PLAN_1_ID, 140m),
            };
            Assert.Equal(expected, recommendations);
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
