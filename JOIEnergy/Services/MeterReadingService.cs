using System;
using System.Collections.Generic;
using JOIEnergy.Domain;
using JOIEnergy.Interfaces;

namespace JOIEnergy.Services
{
    public class MeterReadingService(Dictionary<string, List<ElectricityReading>> meterAssociatedReadings)
        : IMeterReadingService
    {
        public Dictionary<string, List<ElectricityReading>> MeterAssociatedReadings { get; set; } = meterAssociatedReadings;

        public List<ElectricityReading> GetReadings(string smartMeterId) {
            if (MeterAssociatedReadings.ContainsKey(smartMeterId)) {
                return MeterAssociatedReadings[smartMeterId];
            }
            return new List<ElectricityReading>();
        }

        public void StoreReadings(string smartMeterId, List<ElectricityReading> electricityReadings) {
            if (!MeterAssociatedReadings.ContainsKey(smartMeterId)) {
                MeterAssociatedReadings.Add(smartMeterId, new List<ElectricityReading>());
            }

            electricityReadings.ForEach(electricityReading => MeterAssociatedReadings[smartMeterId].Add(electricityReading));
        }
    }
}
