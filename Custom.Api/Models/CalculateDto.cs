using System;
using Custom.BL.Enums;

namespace Custom.Api.Models
{
    public class CalculateDto
    {
        public CarType CarType { get; set; }
       
        public FuelType FuelType { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }

        public int FuelWeight { get; set; }
    }
}
