using System;
namespace JOIEnergy.Domain
{
    public class ElectricityReading
    {
        public DateTime Time { get; set; }
        public decimal Reading { get; set; }
    }
}
