using Custom.BL.Enums;
using Custom.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Custom.BL.Models;

namespace Custom.Cmd
{
    public class ProcessLogicUI
    {
        private static readonly Dictionary<ConsoleKey, string> CommandsNames = new Dictionary<ConsoleKey, string>
        {
            {ConsoleKey.C, "Car"},
            {ConsoleKey.T, "Truck"},
            {ConsoleKey.B, "Bike"},
            {ConsoleKey.E, "Exit"},
        };

        /// <summary>
        /// Instance of <see cref="ICustomService"/>
        /// </summary>
        private static ICustomService _customService;

        /// <summary>
        /// Service that calculate custom of cars.
        /// </summary>
        private static ICustomService CustomService =>
            _customService ??= new CustomCalculatorService();

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
            Console.WriteLine("Enter the fuel type: ");

            ShowFuelTypes();

            var fuelType = Parsing.ParseFuelType();

            if (fuelType == FuelType.Electric)
            {
                var carEnginePower = Parsing.ParseInt("engine power in KW");

                var electricCarResult = CustomService.GetResult(new CalculateModel
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

                var carResult = CustomService.GetResult(new CalculateModel
                {
                    CarType = CarType.Car,
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

            var truckResult = CustomService.GetResult(new CalculateModel
            {
                CarType = CarType.Truck,
                EngineVolume = truckEngineVolume,
                FuelWeight = truckFullWeight,
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

            var bikeResult = CustomService.GetResult(new CalculateModel
            {
                CarType = CarType.Bike,
                Price = bikePrice,
                Year = bikeYear,
                EngineVolume = bikeEngineVolume,
            });

            Console.WriteLine($"Full payment : {bikeResult} EUR");
        }

        public static void SayHello() =>
            Console.WriteLine("Hello! Welcome to the customs calculator");

        public static void SayBye() =>
            Console.WriteLine("Bye!");

        public static void ShowCommands()
        {
            Console.WriteLine("Choose your vehicle:");

            foreach (var key in CommandsNames.Keys)
                Console.WriteLine($"{key} - {CommandsNames[key]}");
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