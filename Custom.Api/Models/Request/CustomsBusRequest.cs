using Custom.DAL.Entities;
using System;

namespace Custom.Api.Models.Request
{
    public class CustomsBusRequest
    {
        public VehicleType VehicleType { get; set; } = VehicleType.Bus;

        public FuelType FuelType { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }
    }
}