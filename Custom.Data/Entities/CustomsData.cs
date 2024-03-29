﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Custom.DAL.Entities
{
    public class CustomsData
    {
        [Key]
        public int Id { get; set; }

        public VehicleType VehicleType { get; set; }

        public FuelType FuelType { get; set; }

        public int EngineVolume { get; set; }

        public int Price { get; set; }

        public DateTime Year { get; set; }

        public int VehicleWeight { get; set; }
    }
}