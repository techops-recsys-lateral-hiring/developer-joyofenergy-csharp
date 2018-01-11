using System;
using System.Collections.Generic;
using JOIEnergy.Domain;
using JOIEnergy.Enums;
using Xunit;

namespace JOIEnergy.Tests
{
    public class PricePlanTest
    {
        private PricePlan _pricePlan;

        public PricePlanTest()
        {
            _pricePlan = new PricePlan
            {
                EnergySupplier = Supplier.TheGreenEco,
                UnitRate = 20m,
                PeakTimeMultiplier = new List<PeakTimeMultiplier> {
                    new PeakTimeMultiplier { 
                        DayOfWeek = DayOfWeek.Saturday,
                        Multiplier = 2m
                    },
                    new PeakTimeMultiplier {
                        DayOfWeek = DayOfWeek.Sunday,
                        Multiplier = 10m
                    }
                }
            };
        }

        [Fact]
        public void TestGetEnergySupplier() {
            Assert.Equal(Supplier.TheGreenEco, _pricePlan.EnergySupplier);
        }

        [Fact]
        public void TestGetBasePrice() {
            Assert.Equal(20m, _pricePlan.GetPrice(new DateTime(2018, 1, 2)));
        }

        [Fact]
        public void TestGetPeakTimePrice()
        {
            Assert.Equal(40m, _pricePlan.GetPrice(new DateTime(2018, 1, 6)));
        }

    }
}
