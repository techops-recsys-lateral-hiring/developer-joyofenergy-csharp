using JOIEnergy.Enums;

namespace JOIEnergy.Services
{
    public interface IAccountService
    {
        Supplier GetPricePlanIdForSmartMeterId(string smartMeterId);
    }
}