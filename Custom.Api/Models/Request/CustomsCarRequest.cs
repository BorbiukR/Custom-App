using Custom.DAL.Entities;
using System;

namespace Custom.Api.Models.Request
{
    public class CustomsCarRequest
    {
        public int Id { get; set; }

        public FuelType FuelType { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }
    }
}