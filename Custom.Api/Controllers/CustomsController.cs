using AutoMapper;
using Custom.Api.Models.Request;
using Custom.BL.Models;
using Custom.BL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Custom.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomsController : ControllerBase
    {
        // TODO: пофіксити формат часу ("dd/mm/yyyy")

        private readonly ILogger<CustomsController> _logger;
        private readonly ICustomsService _service;
        private readonly IMapper _mapper;

        public CustomsController(ILogger<CustomsController> logger, ICustomsService service, IMapper mapper)
        {
            _logger = logger;
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("calculate/car/electric")]
        public IActionResult CalculateForElectricCar([FromBody] CustomsElectricCarRequest request) 
        {
            try
            {
                var res = _service.GetResult(_mapper.Map<CustomsDataDTO>(request));

                _logger.LogInformation("Calculate customs for electric car successfully");

                return StatusCode(200, $"The sum of all payments: {res} Euro");
            }
            catch (Exception)
            {
                _logger.LogInformation("Calculate customs for electric car NOT successfully");

                return StatusCode(500, "Model is not valid. Check the input");
            }
        }

        [HttpPost("calculate/car")]
        public IActionResult CalculateForCar([FromBody] CustomsCarRequest request)
        {
            try
            {
                var res = _service.GetResult(_mapper.Map<CustomsDataDTO>(request));

                _logger.LogInformation("Calculate customs for car successfully");

                return StatusCode(200, $"The sum of all payments: {res} Euro");
            }
            catch (Exception)
            {
                _logger.LogInformation("Calculate customs for car NOT successfully");

                return StatusCode(500, "Model is not valid. Check the input");
            }
        }

        [HttpPost("calculate/bus")]
        public IActionResult CalculateForBus([FromBody] CustomsBusRequest request)
        {
            try
            {
                var res = _service.GetResult(_mapper.Map<CustomsDataDTO>(request));

                _logger.LogInformation("Calculate customs for bus successfully");

                return StatusCode(200, $"The sum of all payments: {res} Euro");
            }
            catch (Exception)
            {
                _logger.LogInformation("Calculate customs for bus NOT successfully");

                return StatusCode(500, "Model is not valid. Check the input");
            }
        }

        [HttpPost("calculate/bike")]
        public IActionResult CalculateForBike([FromBody] CustomsBikeRequest request)
        {
            try
            {
                var res = _service.GetResult(_mapper.Map<CustomsDataDTO>(request));

                _logger.LogInformation("Calculate customs for bike successfully");

                return StatusCode(200, $"The sum of all payments: {res} Euro");
            }
            catch (Exception)
            {
                _logger.LogInformation("Calculate customs for bike NOT successfully");

                return StatusCode(500, "Model is not valid. Check the input");
            }
        }

        [HttpPost("calculate/truck")]
        public IActionResult CalculateForTruck([FromBody] CustomsTruckRequest request)
        {
            try
            {
                var res = _service.GetResult(_mapper.Map<CustomsDataDTO>(request));

                _logger.LogInformation("Calculate customs for truck successfully");

                return StatusCode(200, $"The sum of all payments: {res} Euro");
            }
            catch (Exception)
            {
                _logger.LogInformation("Calculate customs for truck NOT successfully");

                return StatusCode(500, "Model is not valid. Check the input");
            }
        }
    }
}