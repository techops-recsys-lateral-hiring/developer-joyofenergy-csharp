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
        private readonly DebugOutputter output;

        public PricePlanComparisonTest(ITestOutputHelper outputHelper)
        {
            var readings = new Dictionary<string, List<Domain.ElectricityReading>>();
            this.output = new DebugOutputter(outputHelper);
            meterReadingService = new MeterReadingService(readings);
            var pricePlan = new PricePlan() { EnergySupplier = Supplier.DrEvilsDarkEnergy, UnitRate = 10, PeakTimeMultiplier = new List<PeakTimeMultiplier>()};
            var otherPricePlan = new PricePlan() { EnergySupplier = Supplier.PowerForEveryone, UnitRate = 1, PeakTimeMultiplier = new List<PeakTimeMultiplier>() };
            var pricePlans = new List<PricePlan>() { pricePlan, otherPricePlan };
            var pricePlanService = new PricePlanService(pricePlans, meterReadingService, output);
            var accountService = new AccountService(smartMeterToPricePlanAccounts);
            controller = new PricePlanComparatorController(pricePlanService, accountService);
        }

        class DebugOutputter : PricePlanService.Debug
        {
            private readonly ITestOutputHelper output;

            public DebugOutputter(ITestOutputHelper output)
            {
                this.output = output;
            }

            public void Log(string s)
            {
                output.WriteLine(s);
            }
        }

        [Fact]
        public void ShouldCalculateCostForMeterReadingsForEveryPricePlan()
        {
            var pricePlanToCost = new Dictionary<string, decimal>();
            pricePlanToCost[Supplier.DrEvilsDarkEnergy.ToString()] = 100.0m;
            pricePlanToCost[Supplier.PowerForEveryone.ToString()] = 10.0m;

            var smartMeterId = "smart-meter-id";
            var electricityReading = new ElectricityReading() { Time = DateTime.Now.AddHours(-1), Reading = 15.0m };
            var otherReading = new ElectricityReading() { Time = DateTime.Now.AddHours(-1), Reading = 5.0m };
            meterReadingService.StoreReadings(smartMeterId, new List<ElectricityReading>() { electricityReading, otherReading });

            var result = controller.CalculatedCostForEachPricePlan(smartMeterId).Value;

            Assert.Equal(pricePlanToCost, ((Newtonsoft.Json.Linq.JObject)result).ToObject<Dictionary<string, decimal>>());
        }

        [Fact]
        public void GivenNoMatchingMeterIdShouldReturnNotFound()
        {
            Assert.Equal(404, controller.CalculatedCostForEachPricePlan("not-found").StatusCode);
        }
    }
}
