using JOIEnergy.Enums;

namespace JOIEnergy.Services
{
    public interface IAccountService
    {
        string GetPricePlanIdForSmartMeterId(string smartMeterId);
    }
}