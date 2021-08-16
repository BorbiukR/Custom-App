using System;

namespace Custom.Api.Models.Request
{
    public class CustomsTruckRequest
    {
        public int Id { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }

        public int VehicleWeight { get; set; }
    }
}