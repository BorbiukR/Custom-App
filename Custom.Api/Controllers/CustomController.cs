using Custom.BL.Models;
using Custom.BL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Custom.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomController : ControllerBase
    {
        private readonly ILogger<CustomController> _logger;
        private readonly ICustomsService _service;

        public CustomController(ILogger<CustomController> logger, ICustomsService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("calculate/car")]
        public async Task<int> CalculateForCar(CustomsDataDTO dto) 
        {
            return await _service.GetResult(new CustomsDataDTO
            {
                VehicleType = dto.VehicleType,
                EngineVolume = dto.EngineVolume,
                FuelType = dto.FuelType,
                VehicleWeight = dto.VehicleWeight,
                Price = dto.Price,
                Year = dto.Year,
            });
        }
    }
}