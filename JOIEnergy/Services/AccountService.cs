using System;
using System.Collections.Generic;
using JOIEnergy.Enums;
using JOIEnergy.Interfaces;

namespace JOIEnergy.Services
{
    public class AccountService(Dictionary<string, Supplier> smartMeterToPricePlanAccounts) : Dictionary<string, Supplier>, IAccountService
    {
        public Supplier GetPricePlanIdForSmartMeterId(string smartMeterId)
        {
            return smartMeterToPricePlanAccounts.GetValueOrDefault(smartMeterId, Supplier.NullSupplier);
        }
    }
}
