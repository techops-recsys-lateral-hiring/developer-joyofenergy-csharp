using System;
namespace JOIEnergy.Domain
{
    public class ElectricityReading
    {
        public DateTime Time { get; set; }
        public Decimal Reading { get; set; }
    }
}
