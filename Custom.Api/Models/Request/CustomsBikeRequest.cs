using Custom.DAL.Entities;
using System;

namespace Custom.Api.Models.Request
{
    public class CustomsBikeRequest
    {
        public VehicleType VehicleType { get; set; } = VehicleType.Bike;

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; } 
    }
}