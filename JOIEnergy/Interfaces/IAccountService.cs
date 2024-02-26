using JOIEnergy.Enums;

namespace JOIEnergy.Interfaces
{
    public interface IAccountService
    {
        Supplier GetPricePlanIdForSmartMeterId(string smartMeterId);
    }
}