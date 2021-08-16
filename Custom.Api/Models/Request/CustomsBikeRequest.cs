using System;

namespace Custom.Api.Models.Request
{
    public class CustomsBikeRequest
    {
        public int Id { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }
    }
}