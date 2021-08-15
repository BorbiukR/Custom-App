using Custom.DAL.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CustomsViewModel
    {
        [Display(Name = "Car Type")]
        public VehicleType CarType { get; set; }


        [Display(Name = "Fuel Type")]
        public FuelType FuelType { get; set; }


        [Display(Name = "Engine Volume")]
        public int EngineVolume { get; set; }


        [Display(Name = "Price")]
        public int Price { get; set; }


        [Display(Name = "Year")]
        public DateTime Year { get; set; }


        [Display(Name = "Car Weight")]
        public int CarWeight { get; set; }


        [Display(Name = "Result")]
        public int Result { get; set; }
    }
}