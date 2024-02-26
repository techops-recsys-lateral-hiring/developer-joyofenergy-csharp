using System;
using System.Collections.Generic;
using JOIEnergy.Enums;
using JOIEnergy.Interfaces;

namespace JOIEnergy.Services
{
    public class AccountService(Dictionary<string, Supplier> smartMeterToPricePlanAccounts) : Dictionary<string, Supplier>, IAccountService
    { 
        private Dictionary<string, Supplier> _smartMeterToPricePlanAccounts = smartMeterToPricePlanAccounts;

        public Supplier GetPricePlanIdForSmartMeterId(string smartMeterId) {
            if (!_smartMeterToPricePlanAccounts.ContainsKey(smartMeterId))
            {
                return Supplier.NullSupplier;
            }
            return _smartMeterToPricePlanAccounts[smartMeterId];
        }
    }
}
