using System;
using Custom.BL.Enums;
using Custom.BL.Models;

namespace Custom.BL.Services
{
    public class CustomCalculatorService : ICustomService
    {
        public int GetResult(CalculateModel model)
        {
            return model.CarType switch
            {
                CarType.Car => GetCarCustomValue(model.FuelType, model.EngineVolume, model.Price, model.Year),
                CarType.Truck => GetTruckCustomValue(model.Price, model.Year, model.EngineVolume, model.FuelWeight),
                CarType.Bus => GetBusCustomValue(model.Price, model.Year, model.EngineVolume, model.FuelType),
                CarType.Bike => GetBikeCustomValue(model.Price, model.Year, model.EngineVolume),
                _ => throw new NotImplementedException()
            };
        }

        private static int GetCarCustomValue(FuelType fuelType, int engineVolume, int price = default, DateTime year = default)
        {
            if (fuelType == FuelType.Electric)
            {
                return engineVolume;
            }

            if (price == default || year == default)
            {
                throw new ArgumentException(
                    "Price and Year are mandatory parameters for not Electric cars Custom calculating");
            }

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

        private static int GetBusCustomValue(int price, DateTime year, int engineVolume, FuelType fuelType)
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
        private static int GetCarExciseValue(DateTime year, FuelType fuelType, int engineVolume)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            var rate = fuelType switch
            {
                FuelType.Diesel => engineVolume < 3000 ? 50 : 100,
                FuelType.Gas => engineVolume < 3500 ? 75 : 150,
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
            {
                if (totalYearsCount < 5) rate = 0.02;
                else if (totalYearsCount > 5 && totalYearsCount < 8) rate = 0.8;
                else if (totalYearsCount > 8) rate = 1;
            }
            else if (fullWeight > 5000)
            {
                if (totalYearsCount < 5) rate = 0.026;
                else if (totalYearsCount > 5 && totalYearsCount < 8) rate = 1.04;
                else if (totalYearsCount > 8) rate = 1.3;
            }

            var res = rate * (engineVolume / 1000) * totalYearsCount;

            return (int) Math.Round(res, 0);
        }

        /// <summary>
        /// Calculates excise duty for bikes 
        /// </summary>
        private static int GetBikeExciseValue(DateTime year, int engineVolume)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            double rate = default;

            if (engineVolume < 500) rate = 0.062;
            else if (engineVolume > 500 && engineVolume < 800) rate = 0.443;
            else if (engineVolume > 800) rate = 0.447;

            var res = rate * (engineVolume / 1000) * totalYearsCount;

            return (int) Math.Round(res, 0);
        }

        /// <summary>
        /// Calculates excise duty for buses
        /// </summary>
        private static int GetBusExciseValue(DateTime year, int engineVolume, FuelType fuelType)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            double rate = default;

            if (fuelType == FuelType.Gas)
            {
                if (totalYearsCount < 8) rate = 0.007;
                else if (totalYearsCount > 8) rate = 0.35;
            }
            else if (fuelType == FuelType.Diesel)
            {
                if (totalYearsCount < 8)
                {
                    if (engineVolume < 2500 && engineVolume > 5000) //TODO: check the expression
                        rate = 0.007;
                    else if (engineVolume > 2500 && engineVolume < 5000)
                        rate = 0.003;
                }
                else if (totalYearsCount > 8)
                {
                    if (engineVolume < 2500 && engineVolume > 5000) //TODO: check the expression
                        rate = 0.35;
                    else if (engineVolume > 2500 && engineVolume < 5000)
                        rate = 0.15;
                }
            }

            var excise = rate * engineVolume / 1000 * totalYearsCount;

            return (int) Math.Round(excise, 0);
        }
    }
}
