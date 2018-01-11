using JOIEnergy.Controllers;
using JOIEnergy.Domain;
using JOIEnergy.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JOIEnergy.Tests
{
    public class PricePlanComparisonTest
    {
        // private PricePlanComparatorController controller;
        // private MeterReadingService meterReadingService;

        private String PRICE_PLAN_1_ID = "test-price-plan";
        private String PRICE_PLAN_2_ID = "best-price-plan";
        private MeterReadingService meterReadingService;
        private PricePlanComparatorController controller;
        private Dictionary<string, Enums.Supplier> smartMeterToPricePlanAccounts = new Dictionary<string, Enums.Supplier>();

        public PricePlanComparisonTest()
        {
            var readings = new Dictionary<string, List<Domain.ElectricityReading>>();
            meterReadingService = new MeterReadingService(readings);
            var pricePlan = new PricePlan() { EnergySupplier = Enums.Supplier.DrEvilsDarkEnergy, UnitRate = 10, PeakTimeMultiplier = new List<PeakTimeMultiplier>()};
            var otherPricePlan = new PricePlan() { EnergySupplier = Enums.Supplier.PowerForEveryone, UnitRate = 1, PeakTimeMultiplier = new List<PeakTimeMultiplier>() };
            var pricePlans = new List<PricePlan>() { pricePlan, otherPricePlan };
            var pricePlanService = new PricePlanService(pricePlans, meterReadingService);
            var accountService = new AccountService(smartMeterToPricePlanAccounts);
            controller = new PricePlanComparatorController(pricePlanService, accountService);
        }

        [Fact]
        public void GivenNoMatchingMeterIdShouldReturnNotFound()
        {
            Assert.Equal(404, controller.CalculatedCostForEachPricePlan("not-found").StatusCode);
        }
    }
}
