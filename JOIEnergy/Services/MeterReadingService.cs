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

        public List<ElectricityReading> GetReadings(string smartMeterId)
        {
            return MeterAssociatedReadings.TryGetValue(smartMeterId, out var reading) ? reading : [];
        }

        public void StoreReadings(string smartMeterId, List<ElectricityReading> electricityReadings) {
            if (!MeterAssociatedReadings.TryGetValue(smartMeterId, out var value)) {
                value = [];
                MeterAssociatedReadings.Add(smartMeterId, value);
            }

            electricityReadings.ForEach(electricityReading => value.Add(electricityReading));
        }
    }
}
