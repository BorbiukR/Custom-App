using AutoMapper;
using Custom.Api.Models.Request;
using Custom.BL.Models;
using Custom.BL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Custom.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomsController : ControllerBase
    {
        private readonly ILogger<CustomsController> _logger;
        private readonly ICustomsService _service;
        private readonly IMapper _mapper;

        public CustomsController(ILogger<CustomsController> logger, ICustomsService service, IMapper mapper)
        {
            _logger = logger;
            _service = service;
            _mapper = mapper;
        }

        //public IActionResult AddToTask(Guid employeeId, [FromBody] EmployeeTaskRequest employeeTask)
        //{
        //    try
        //    {
        //        employeeTask.EmployeeId = employeeId;
        //        var added = employeeService.AddToTask(mapper.Map<EmployeeTaskDto>(employeeTask));
        //        if (!added)
        //        {
        //            return StatusCode(404, "Task not found");
        //        }
        //        return StatusCode(201, "Added");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}

        [HttpPost("calculate/car/electric")]
        public async Task<int> CalculateForElectricCar([FromBody] CustomsElectricCarRequest request) 
        {
            return await _service.GetResult(new CustomsDataDTO
            {
                Id = request.Id,
                EngineVolume = request.EngineVolume,
                FuelType = request.FuelType,
            });
        }

        [HttpPost("calculate/car")]
        public async Task<int> CalculateForCar([FromBody] CustomsCarRequest request)
        {
            return await _service.GetResult(new CustomsDataDTO
            {
                Id = request.Id,
                EngineVolume = request.EngineVolume,
                FuelType = request.FuelType,
                Price = request.Price,
                Year = request.Year,
            });
        }

        [HttpPost("calculate/bus")]
        public async Task<int> CalculateForBus([FromBody] CustomsBusRequest request)
        {
            return await _service.GetResult(new CustomsDataDTO
            {
                Id = request.Id,
                EngineVolume = request.EngineVolume,
                FuelType = request.FuelType,
                Price = request.Price,
                Year = request.Year,
            });
        }

        [HttpPost("calculate/bike")]
        public async Task<int> CalculateForBike([FromBody] CustomsBikeRequest request)
        {
            return await _service.GetResult(new CustomsDataDTO
            {
                Id = request.Id,
                EngineVolume = request.EngineVolume,
                Price = request.Price,
                Year = request.Year,
            });
        }

        [HttpPost("calculate/truck")]
        public async Task<int> CalculateForTruck([FromBody] CustomsTruckRequest request)
        {
            return await _service.GetResult(new CustomsDataDTO
            {
                Id = request.Id,
                EngineVolume = request.EngineVolume,
                VehicleWeight = request.VehicleWeight,
                Price = request.Price,
                Year = request.Year,
            });
        }
    }
}