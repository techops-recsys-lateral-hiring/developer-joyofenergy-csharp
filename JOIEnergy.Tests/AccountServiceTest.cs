using System;
using System.Collections.Generic;
using JOIEnergy.Enums;
using JOIEnergy.Services;
using Xunit;

namespace JOIEnergy.Tests
{
    public class AccountServiceTest
    {
        private const Supplier PRICE_PLAN_ID = Supplier.PowerForEveryone;
        private const String SMART_METER_ID = "smart-meter-id";

        private AccountService accountService;

        public AccountServiceTest()
        {
            Dictionary<String, Supplier> smartMeterToPricePlanAccounts = new Dictionary<string, Supplier>();
            smartMeterToPricePlanAccounts.Add(SMART_METER_ID, PRICE_PLAN_ID);

            accountService = new AccountService(smartMeterToPricePlanAccounts);
        }

        [Fact]
        public void GivenTheSmartMeterIdReturnsThePricePlanId()
        {
            var result = accountService.GetPricePlanIdForSmartMeterId("smart-meter-id");
            Assert.Equal(Supplier.PowerForEveryone, result);
        }

        [Fact]
        public void GivenAnUnknownSmartMeterIdReturnsANullSupplier()
        {
            var result = accountService.GetPricePlanIdForSmartMeterId("bob-carolgees");
            Assert.Equal(Supplier.NullSupplier, result);
        }
    }
}
