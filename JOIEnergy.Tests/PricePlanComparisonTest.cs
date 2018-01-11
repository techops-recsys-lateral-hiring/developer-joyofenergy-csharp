using JOIEnergy.Controllers;
using JOIEnergy.Domain;
using JOIEnergy.Enums;
using JOIEnergy.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

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

            var actualCosts = ((Newtonsoft.Json.Linq.JObject)result).ToObject<Dictionary<string, decimal>>();
            Assert.Equal(3, actualCosts.Count);
            Assert.Equal(100m, actualCosts["" + Supplier.DrEvilsDarkEnergy], 3);
            Assert.Equal(20m, actualCosts["" + Supplier.TheGreenEco], 3);
            Assert.Equal(10m, actualCosts["" + Supplier.PowerForEveryone], 3);
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
