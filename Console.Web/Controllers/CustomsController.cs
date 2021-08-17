using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Custom.BL.Models;
using Custom.BL.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class CustomsController : Controller
    {
        private readonly ICustomsService _customService;

        public CustomsController(ICustomsService customService)
        {
            _customService = customService;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(CustomsViewModel model)
        {
            model.Result =  _customService.GetResult(new CustomsDataDTO
            {
                VehicleType = model.VehicleType,
                EngineVolume = model.EngineVolume,
                FuelType = model.FuelType,
                VehicleWeight = model.VehicleWeight,
                Price = model.Price,
                Year = model.Year,
            });

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}