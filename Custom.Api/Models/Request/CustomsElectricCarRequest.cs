using Custom.DAL.Entities;

namespace Custom.Api.Models.Request
{
    public class CustomsElectricCarRequest
    {
        public VehicleType VehicleType { get; set; } = VehicleType.ElectricCar;

        public FuelType FuelType { get; set; } = FuelType.Electric;

        public int EngineVolume { get; set; }
    }
}