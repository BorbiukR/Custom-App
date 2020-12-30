using System;
using System.ComponentModel.DataAnnotations;
using Custom.BL.Enums;

namespace Console.Web.Models
{
    public class CustomViewModel
    {
        [Display(Name = "Car Type")]
        public CarType CarType { get; set; }

        [Display(Name = "Fuel Type")]
        public FuelType FuelType { get; set; }

        [Display(Name = "Engine Volume")]
        public int EngineVolume { get; set; }

        [Display(Name = "Price")]
        public int Price { get; set; }

        [Display(Name = "Year")]
        public DateTime Year { get; set; }

        [Display(Name = "Fuel Weight")]
        public int FuelWeight { get; set; }

        [Display(Name = "Result")]
        public int Result { get; set; }
    }
}
