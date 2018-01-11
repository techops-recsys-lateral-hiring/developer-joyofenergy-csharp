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
        private Dictionary<string, Enums.Supplier> smartMeterToPricePlanAccounts = new Dictionary<string, Enums.Supplier>();

        public PricePlanComparisonTest()
        {
            var readings = new Dictionary<string, List<Domain.ElectricityReading>>();
            meterReadingService = new MeterReadingService(readings);
            var pricePlan = new PricePlan() { EnergySupplier = Supplier.DrEvilsDarkEnergy, UnitRate = 10, PeakTimeMultiplier = new List<PeakTimeMultiplier>()};
            var otherPricePlan = new PricePlan() { EnergySupplier = Supplier.PowerForEveryone, UnitRate = 1, PeakTimeMultiplier = new List<PeakTimeMultiplier>() };
            var pricePlans = new List<PricePlan>() { pricePlan, otherPricePlan };
            var pricePlanService = new PricePlanService(pricePlans, meterReadingService);
            var accountService = new AccountService(smartMeterToPricePlanAccounts);
            controller = new PricePlanComparatorController(pricePlanService, accountService);
        }

        [Fact]
        public void ShouldCalculateCostForMeterReadingsForEveryPricePlan()
        {
            var smartMeterId = "smart-meter-id";
            var electricityReading = new ElectricityReading() { Time = DateTime.Now.AddHours(-1), Reading = 15.0m };
            var otherReading = new ElectricityReading() { Time = DateTime.Now, Reading = 5.0m };
            meterReadingService.StoreReadings(smartMeterId, new List<ElectricityReading>() { electricityReading, otherReading });

            var result = controller.CalculatedCostForEachPricePlan(smartMeterId).Value;

            var actualCosts = ((Newtonsoft.Json.Linq.JObject)result).ToObject<Dictionary<string, decimal>>();
            Assert.Equal(2, actualCosts.Count);
            Assert.Equal(100.0m, actualCosts["" + Supplier.DrEvilsDarkEnergy], 3);
            Assert.Equal(10.0m, actualCosts["" + Supplier.PowerForEveryone], 3);
        }

        [Fact]
        public void GivenNoMatchingMeterIdShouldReturnNotFound()
        {
            Assert.Equal(404, controller.CalculatedCostForEachPricePlan("not-found").StatusCode);
        }
    }
}
