﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Custom.BL.Models;
using Custom.BL.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class CustomController : Controller
    {
        private readonly ICustomService _customService;

        public CustomController(ICustomService customService)
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
            model.Result = _customService.GetResult(new CalculateDTO
            {
                CarType = model.CarType,
                EngineVolume = model.EngineVolume,
                FuelType = model.FuelType,
                CarWeight = model.CarWeight,
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