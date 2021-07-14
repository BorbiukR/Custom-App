using System;
using Custom.BL.Enums;
using Custom.BL.Models;

namespace Custom.BL.Services
{
    public class CustomCalculatorService : ICustomService
    {
        public int GetResult(CalculateDTO model)
        {
            return model.CarType switch
            {
                CarTypeDTO.Car => GetCarCustomValue(model.FuelType, model.EngineVolume, model.Price, model.Year),
                CarTypeDTO.Truck => GetTruckCustomValue(model.Price, model.Year, model.EngineVolume, model.CarWeight),
                CarTypeDTO.Bus => GetBusCustomValue(model.Price, model.Year, model.EngineVolume, model.FuelType),
                CarTypeDTO.Bike => GetBikeCustomValue(model.Price, model.Year, model.EngineVolume),
                _ => throw new NotImplementedException()
            };
        }

        private static int GetCarCustomValue(FuelTypeDTO fuelType, int engineVolume, int price = default, DateTime year = default)
        {
            if (fuelType == FuelTypeDTO.Electric)
                return engineVolume;

            if (price == default || year == default)
                throw new ArgumentException(
                    "Price and Year are mandatory parameters for not Electric cars Custom calculating");

            var importDuty = GetImportDuty(price);
            var exciseValue = GetCarExciseValue(year, fuelType, engineVolume);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private static int GetTruckCustomValue(int price, DateTime year, int engineVolume, int fullWeight)
        {
            var importDuty = GetImportDuty(price);
            var exciseValue = GetTruckExciseValue(year, fullWeight, engineVolume);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private static int GetBikeCustomValue(int price, DateTime year, int engineVolume)
        {
            var importDuty = GetImportDuty(price);
            var exciseValue = GetBikeExciseValue(year, engineVolume);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private static int GetBusCustomValue(int price, DateTime year, int engineVolume, FuelTypeDTO fuelType)
        {
            var importDuty = GetImportDuty(price);
            var exciseValue = GetBusExciseValue(year, engineVolume, fuelType);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private static int GetImportDuty(int price) => price / 10;

        private static int GetVat(int price, int importDuty, int exciseValue) =>
            Convert.ToInt32((price + importDuty + exciseValue) * 0.2);

        private static int GetFullPayment(int exciseTax, int importDuty, int vat) =>
            Convert.ToInt32(exciseTax + importDuty + vat);

        /// <summary>
        /// Counts the number of full years 
        /// </summary>
        private static int GetCountOfFullYears(DateTime year)
        {
            var now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            var dob = int.Parse(year.ToString("yyyyMMdd"));
            return (now - dob) / 10000;
        }

        /// <summary>
        /// Calculates excise duty for cars 
        /// </summary>
        private static int GetCarExciseValue(DateTime year, FuelTypeDTO fuelType, int engineVolume)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            var rate = fuelType switch
            {
                FuelTypeDTO.Diesel => engineVolume < 3000 ? 50 : 100,
                FuelTypeDTO.Gas => engineVolume < 3500 ? 75 : 150,
                _ => throw new ArgumentException("Invalid Fuel Type for Rate calculating")
            };

            rate = totalYearsCount > 15
                ? rate * 15
                : rate * totalYearsCount;

            var res = rate * engineVolume / 1000.0;

            return (int) Math.Round(res, 0);
        }

        /// <summary>
        /// Calculates excise duty for trucks
        /// </summary>
        private static int GetTruckExciseValue(DateTime year, int fullWeight, int engineVolume)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            double rate = default;

            if (fullWeight < 5000)
                rate = GetRateForTruckWhereWightMoreThan5000(totalYearsCount, rate);

            if (fullWeight > 5000)
                rate = GetRateForTruckWhereWightLessThan5000(totalYearsCount, rate);

            var res = rate * (engineVolume / 1000) * totalYearsCount;

            return (int) Math.Round(res, 0);
        }

        private static double GetRateForTruckWhereWightMoreThan5000(int totalYearsCount, double rate)
        {
            if (totalYearsCount < 5) rate = 0.02;
            if (totalYearsCount > 5 && totalYearsCount < 8) rate = 0.8;
            if (totalYearsCount > 8) rate = 1;
           
            return rate;
        }

        private static double GetRateForTruckWhereWightLessThan5000(int totalYearsCount, double rate)
        {
            if (totalYearsCount < 5) rate = 0.026;
            if (totalYearsCount > 5 && totalYearsCount < 8) rate = 1.04;
            if (totalYearsCount > 8) rate = 1.3;

            return rate;
        }

        /// <summary>
        /// Calculates excise duty for bikes 
        /// </summary>
        private static int GetBikeExciseValue(DateTime year, int engineVolume)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            double rate = default;

            if (engineVolume < 500) rate = 0.062;
            if (engineVolume > 500 && engineVolume < 800) rate = 0.443;
            if (engineVolume > 800) rate = 0.447;

            var res = rate * (engineVolume / 1000) * totalYearsCount;

            return (int) Math.Round(res, 0);
        }

        /// <summary>
        /// Calculates excise duty for buses
        /// </summary>
        private static int GetBusExciseValue(DateTime year, int engineVolume, FuelTypeDTO fuelType)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            double rate = default;

            if (fuelType == FuelTypeDTO.Gas)
            {
                rate = totalYearsCount < 8 
                    ? 0.007 
                    : 0.35;
            }

            if (fuelType == FuelTypeDTO.Diesel)
            {
                rate = GetRateForBusVehicleType(engineVolume, totalYearsCount, rate);
            }

            var excise = rate * engineVolume / 1000 * totalYearsCount;

            return (int) Math.Round(excise, 0);            
        }

        private static double GetRateForBusVehicleType(int engineVolume, int totalYearsCount, double rate)
        {
            if (totalYearsCount < 8)
            {
                rate = (engineVolume < 2500 && engineVolume > 5000)
                    ? 0.007
                    : 0.003;
            }

            if (totalYearsCount > 8)
            {
                rate = (engineVolume < 2500 && engineVolume > 5000)
                    ? 0.35
                    : 0.15;
            }

            return rate;
        }
    }
}