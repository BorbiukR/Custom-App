using System;
using System.ComponentModel.DataAnnotations;

namespace Custom.DAL.Entities
{
    public class Calculate
    {
        [Key]
        public int CalculateId { get; set; }

        public CarType CarType { get; set; }

        public FuelType FuelType { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }

        public int CarWeight { get; set; }
    }
}
