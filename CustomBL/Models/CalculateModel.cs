using System;
using Custom.BL.Enums;

namespace Custom.BL.Models
{
    public class CalculateModel
    {
        public CarType CarType { get; set; }
       
        public FuelType FuelType { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }

        public int FuelWeight { get; set; }
    }
}
