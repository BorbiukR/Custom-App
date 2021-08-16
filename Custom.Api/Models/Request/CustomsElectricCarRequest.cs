using Custom.DAL.Entities;

namespace Custom.Api.Models.Request
{
    public class CustomsElectricCarRequest
    {
        public int Id { get; set; }

        public FuelType FuelType { get; set; }

        public int EngineVolume { get; set; }
    }
}