using System;
using System.Collections.Generic;
using JOIEnergy.Enums;
using JOIEnergy.Services;
using Xunit;

namespace JOIEnergy.Tests
{
    public class AccountServiceTest
    {
        private const string PRICE_PLAN_ID = "price-plan-id";
        private const string SMART_METER_ID = "smart-meter-id";

        private AccountService accountService;

        public AccountServiceTest()
        {
            var smartMeterToPricePlanAccounts = new Dictionary<string, string>();
            smartMeterToPricePlanAccounts.Add(SMART_METER_ID, PRICE_PLAN_ID);

            accountService = new AccountService(smartMeterToPricePlanAccounts);
        }

        [Fact]
        public void GivenTheSmartMeterIdReturnsThePricePlanId()
        {
            var result = accountService.GetPricePlanIdForSmartMeterId(SMART_METER_ID);
            Assert.Equal(PRICE_PLAN_ID, result);
        }
        
        [Fact]
        public void GivenAnUnknownSmartMeterIdReturnsNull()
        {
            var result = accountService.GetPricePlanIdForSmartMeterId("non-existent");
            Assert.Null(result);
        }

    }
}
