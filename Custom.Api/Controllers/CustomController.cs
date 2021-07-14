using Custom.BL.Models;
using Custom.BL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Custom.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomController : ControllerBase
    {
        private readonly ILogger<CustomController> _logger;
        private readonly ICustomService _service;

        public CustomController(ILogger<CustomController> logger, ICustomService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("calculate")]
        public int Calculate(CalculateDTO dto) 
        {
            return _service.GetResult(new CalculateDTO
            {
                CarType = dto.CarType,
                EngineVolume = dto.EngineVolume,
                FuelType = dto.FuelType,
                CarWeight = dto.CarWeight,
                Price = dto.Price,
                Year = dto.Year,
            });
        }
    }
}