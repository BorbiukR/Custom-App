using Custom.DAL.Entities;
using System;

namespace Custom.Api.Models.Request
{
    public class CustomsTruckRequest
    {
        public VehicleType VehicleType { get; set; } = VehicleType.Truck;

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }

        public int VehicleWeight { get; set; }
    }
}