using System;
using System.Collections.Generic;
using JOIEnergy.Services;
using JOIEnergy.Domain;
using Xunit;
using Moq;

namespace JOIEnergy.Tests
{
    public class MeterReadingServiceTestneo
    {
        private readonly string SMART_METER_ID = "1";

        [Theory]
        [InlineData(0)] // Teste para ID de medidor que não existe
        [InlineData(1)] // Teste para ID de medidor que existe
        public void GetReadings_ReturnsExpectedReadings(int readingsCount)
        {
            // Arrange
            var meterReadingService = GetMockMeterReadingService(readingsCount);

            // Act
            var electricityReadings = meterReadingService.GetReadings(SMART_METER_ID);

            // Assert
            Assert.Equal(readingsCount, electricityReadings.Count);
        }

        [Fact]
        public void StoreReadings_StoresReadings()
        {
            // Arrange SMART_METER_ID
            var meterReadingService = GetMockMeterReadingService(Convert.ToInt32(SMART_METER_ID));
            var readingsToStore = new List<ElectricityReading>
            {
                new ElectricityReading() { Time = DateTime.Now, Reading = 30m }
            };

            // Act
            meterReadingService.StoreReadings(SMART_METER_ID, readingsToStore);

            // Assert
            var storedReadings = meterReadingService.GetReadings(SMART_METER_ID);
            Assert.Equal(readingsToStore, storedReadings);
        }

        [Fact]
        public void StoreReadings_ThrowsException_WhenMeterIdIsNull()
        {
            // Arrange
            var meterReadingService = new MeterReadingService(new Dictionary<string, List<ElectricityReading>>());
            var readingsToStore = new List<ElectricityReading>
            {
                new ElectricityReading() { Time = DateTime.Now, Reading = 30m }
            };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => meterReadingService.StoreReadings(null, readingsToStore));
        }

        private IMeterReadingService GetMockMeterReadingService(int readingsCount)
        {
            var mockMeterReadingService = new Mock<IMeterReadingService>();
            var electricityReadings = new List<ElectricityReading>();

            for (int i = 0; i < readingsCount; i++)
            {
                electricityReadings.Add(new ElectricityReading() { Time = DateTime.Now.AddMinutes(-i * 15), Reading = 30m + i });
            }

            mockMeterReadingService.Setup(m => m.GetReadings(SMART_METER_ID)).Returns(electricityReadings);

            return mockMeterReadingService.Object;
        }
    }
}
