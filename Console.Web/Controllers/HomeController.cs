using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Console.Web.Models;
using Custom.BL.Models;
using Custom.BL.Services;

namespace Console.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomService _customService;

        public HomeController(ICustomService customService)
        {
            _customService = customService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(CustomViewModel model)
        {
            model.Result = _customService.GetResult(new CalculateModel
            {
                CarType = model.CarType,
                EngineVolume = model.EngineVolume,
                FuelType = model.FuelType,
                FuelWeight = model.FuelWeight,
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
