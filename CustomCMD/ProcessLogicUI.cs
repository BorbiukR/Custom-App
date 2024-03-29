﻿using Custom.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Custom.BL.Models;
using Custom.DAL.Entities;

namespace Custom.Cmd
{
    public class ProcessLogicUI
    {
        private static ICustomsService _customService;

        public ProcessLogicUI(ICustomsService customService)
        {
            _customService = customService;
        }
            
        /// <summary>
        /// Service that calculate custom of cars.
        /// </summary>
        //private static ICustomsService CustomService => _customService ??= new CustomCalculatorService();

        private static readonly Dictionary<ConsoleKey, string> _commandsNames = new Dictionary<ConsoleKey, string>
        {
            {ConsoleKey.C, "Car"},
            {ConsoleKey.T, "Truck"},
            {ConsoleKey.B, "Bike"},
            {ConsoleKey.E, "Exit"},
        };

        public static bool Process()
        {
            var command = Console.ReadKey().Key;
            switch (command)
            {
                case ConsoleKey.C:
                    SetParamsAndGetCarCustomResult();
                    return true;
                case ConsoleKey.T:
                    SetParamsAndGetTruckCustomResult();
                    return true;
                case ConsoleKey.B:
                    SetParamsAndGetBikeCustomResult();
                    return true;
                case ConsoleKey.E:
                    return false;
                default:
                    Console.WriteLine("Invalid key");
                    return true;
            }
        }

        private static void SetParamsAndGetCarCustomResult()
        {
            Console.WriteLine("Enter the fuel type: \n");

            ShowFuelTypes();

            var fuelType = Parsing.ParseFuelType();

            if (fuelType == FuelType.Electric)
            {
                var carEnginePower = Parsing.ParseInt("engine power in KW");

                var electricCarResult = _customService.GetResult(new CustomsDataDTO
                {
                    FuelType = fuelType,
                    EngineVolume = carEnginePower,
                });

                Console.WriteLine($"Full payment : {electricCarResult} EUR.");
            }
            else
            {
                var carPrice = Parsing.ParsePrice("price");
                var carYear = Parsing.ParseDateTime("year");
                var carEngineVolume = Parsing.ParseInt("engine volume in cubic centimeters");

                var carResult = _customService.GetResult(new CustomsDataDTO
                {
                    VehicleType = VehicleType.Car,
                    EngineVolume = carEngineVolume,
                    FuelType = fuelType,
                    Year = carYear,
                    Price = carPrice,
                });

                Console.WriteLine($"Full payment : {carResult} EUR.");
            }
        }

        private static void SetParamsAndGetTruckCustomResult()
        {
            var truckPrice = Parsing.ParsePrice("price");
            var truckYear = Parsing.ParseDateTime("year");
            var truckEngineVolume = Parsing.ParseInt("engine volume in cubic centimeters");
            var truckFullWeight = Parsing.ParseInt("full weight in kilograms");

            var truckResult = _customService.GetResult(new CustomsDataDTO
            {
                VehicleType = VehicleType.Truck,
                EngineVolume = truckEngineVolume,
                VehicleWeight = truckFullWeight,
                Price = truckPrice,
                Year = truckYear,
            });

            Console.WriteLine($"Full payment : {truckResult} EUR");
        }

        private static void SetParamsAndGetBikeCustomResult()
        {
            var bikePrice = Parsing.ParsePrice("price");
            var bikeYear = Parsing.ParseDateTime("year");
            var bikeEngineVolume = Parsing.ParseInt("engine volume in cubic centimeters");

            var bikeResult = _customService.GetResult(new CustomsDataDTO
            {
                VehicleType = VehicleType.Bike,
                Price = bikePrice,
                Year = bikeYear,
                EngineVolume = bikeEngineVolume,
            });

            Console.WriteLine($"Full payment : {bikeResult} EUR");
        }

        public static void SayHello() => Console.WriteLine("Hello! Welcome to the customs calculator \n");

        public static void SayBye() => Console.WriteLine("\n Bye!");

        public static void ShowCommands()
        {
            Console.WriteLine("Choose your vehicle:");

            foreach (var key in _commandsNames.Keys)
                Console.WriteLine($"{key} - {_commandsNames[key]}");
        }

        private static void ShowFuelTypes()
        {
            var fuelTypes = Enum.GetValues(typeof(FuelType)).Cast<FuelType>();

            foreach (var fuelType in fuelTypes)
            {
                var fuelTypeValue = (int)fuelType;
                int key = fuelTypeValue switch
                {
                    1 => (int)FuelType.Diesel,
                    2 => (int)FuelType.Gas,
                    3 => (int)FuelType.Electric,
                    _ => throw new ArgumentException("Invalid Fuel Type number")
                };

                Console.WriteLine($"{key} - {fuelType}");
            }
        }
    }
}