using System;
using Custom.BL.Enums;

namespace Custom.BL.Models
{
    public class CalculateDTO
    {
        public int CalculateDTOId { get; set; }

        public CarTypeDTO CarType { get; set; }     
        
        public FuelTypeDTO FuelType { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }

        public int CarWeight { get; set; }
    }
}