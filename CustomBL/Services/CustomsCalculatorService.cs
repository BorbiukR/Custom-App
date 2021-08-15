using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Custom.BL.Models;
using Custom.BL.Validation;
using Custom.DAL.Entities;
using Custom.DAL.Interfaces;

namespace Custom.BL.Services
{
    public class CustomsCalculatorService : ICustomsService
    {
        public readonly IMapper _mapper;
        public readonly IUnitOfWork _unit;

        public CustomsCalculatorService(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        public async Task<int> GetResult(CustomsDataDTO customsData)
        {
            ValidationAllProperties(customsData);

            var mappedCustomsData = _mapper.Map<CustomsData>(customsData);
            await _unit.CustomsRepository.AddAsync(mappedCustomsData);
            await _unit.SaveAsync();

            return customsData.VehicleType switch
            {
                VehicleType.Car => GetCarCustomValue(customsData.FuelType, customsData.EngineVolume, customsData.Price, customsData.Year),
                VehicleType.Truck => GetTruckCustomValue(customsData.Price, customsData.Year, customsData.EngineVolume, customsData.VehicleWeight),
                VehicleType.Bus => GetBusCustomValue(customsData.Price, customsData.Year, customsData.EngineVolume, customsData.FuelType),
                VehicleType.Bike => GetBikeCustomValue(customsData.Price, customsData.Year, customsData.EngineVolume),
                _ => throw new NotImplementedException()
            };      
        }

        private int GetCarCustomValue(FuelType fuelType, int engineVolume, int price = default, DateTime year = default)
        {
            if (fuelType == FuelType.Electric)
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

        private int GetTruckCustomValue(int price, DateTime year, int engineVolume, int fullWeight)
        {
            var importDuty = GetImportDuty(price);
            var exciseValue = GetTruckExciseValue(year, fullWeight, engineVolume);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private int GetBikeCustomValue(int price, DateTime year, int engineVolume)
        {
            var importDuty = GetImportDuty(price);
            var exciseValue = GetBikeExciseValue(year, engineVolume);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private int GetBusCustomValue(int price, DateTime year, int engineVolume, FuelType fuelType)
        {
            var importDuty = GetImportDuty(price);
            var exciseValue = GetBusExciseValue(year, engineVolume, fuelType);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private int GetImportDuty(int price) => price / 10;

        private int GetVat(int price, int importDuty, int exciseValue) =>
            Convert.ToInt32((price + importDuty + exciseValue) * 0.2);

        private int GetFullPayment(int exciseTax, int importDuty, int vat) =>
            Convert.ToInt32(exciseTax + importDuty + vat);

        /// <summary>
        /// Counts the number of full years 
        /// </summary>
        private int GetCountOfFullYears(DateTime year)
        {
            var now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            var dob = int.Parse(year.ToString("yyyyMMdd"));
            return (now - dob) / 10000;
        }

        /// <summary>
        /// Calculates excise duty for cars 
        /// </summary>
        private int GetCarExciseValue(DateTime year, FuelType fuelType, int engineVolume)
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

            return (int)Math.Round(res, 0);
        }

        /// <summary>
        /// Calculates excise duty for trucks
        /// </summary>
        private int GetTruckExciseValue(DateTime year, int fullWeight, int engineVolume)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            double rate = default;

            if (fullWeight < 5000)
                rate = GetRateForTruckWhereWightMoreThan5000(totalYearsCount, rate);

            if (fullWeight > 5000)
                rate = GetRateForTruckWhereWightLessThan5000(totalYearsCount, rate);

            var res = rate * (engineVolume / 1000) * totalYearsCount;

            return (int)Math.Round(res, 0);
        }

        private double GetRateForTruckWhereWightMoreThan5000(int totalYearsCount, double rate)
        {
            if (totalYearsCount < 5) rate = 0.02;
            if (totalYearsCount > 5 && totalYearsCount < 8) rate = 0.8;
            if (totalYearsCount > 8) rate = 1;

            return rate;
        }

        private double GetRateForTruckWhereWightLessThan5000(int totalYearsCount, double rate)
        {
            if (totalYearsCount < 5) rate = 0.026;
            if (totalYearsCount > 5 && totalYearsCount < 8) rate = 1.04;
            if (totalYearsCount > 8) rate = 1.3;

            return rate;
        }

        /// <summary>
        /// Calculates excise duty for bikes 
        /// </summary>
        private int GetBikeExciseValue(DateTime year, int engineVolume)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            double rate = default;

            if (engineVolume < 500) rate = 0.062;
            if (engineVolume > 500 && engineVolume < 800) rate = 0.443;
            if (engineVolume > 800) rate = 0.447;

            var res = rate * (engineVolume / 1000) * totalYearsCount;

            return (int)Math.Round(res, 0);
        }

        /// <summary>
        /// Calculates excise duty for buses
        /// </summary>
        private int GetBusExciseValue(DateTime year, int engineVolume, FuelType fuelType)
        {
            var totalYearsCount = GetCountOfFullYears(year);

            double rate = default;

            if (fuelType == FuelType.Gas)
            {
                rate = totalYearsCount < 8
                    ? 0.007
                    : 0.35;
            }

            if (fuelType == FuelType.Diesel)
                rate = GetRateForBusVehicleType(engineVolume, totalYearsCount, rate);

            var excise = rate * engineVolume / 1000 * totalYearsCount;

            return (int)Math.Round(excise, 0);
        }

        private double GetRateForBusVehicleType(int engineVolume, int totalYearsCount, double rate)
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

        private void ValidationAllProperties(CustomsDataDTO model)
        {
            if (model.Id == 0 ||
                model.Price == 0 || model.Price > 100_000_000 ||
                model.VehicleWeight < 50 || model.VehicleWeight > 40_000 ||
                model.EngineVolume < 10 || model.EngineVolume > 15000 ||
                model.Year == DateTime.Now || model.Year < new DateTime(1800, 1, 1) ||
                model.VehicleType == 0 || model.FuelType == 0)
            {
                throw new CustomsException("Calculate model is not valid");
            }
        }

        public IEnumerable<CustomsDataDTO> GetAll()
        {
            var customsData = _unit.CustomsRepository.GetAll().ToList();

            if (customsData == null)
                return null;

            return _mapper.Map<IEnumerable<CustomsDataDTO>>(customsData);
        }

        public async Task<CustomsDataDTO> GetByIdAsync(int customsDataId)
        {
            var book = await _unit.CustomsRepository.GetById(customsDataId);

            if (book == null)
                return null;

            return _mapper.Map<CustomsDataDTO>(book);
        }

        public Task UpdateAsync(CustomsDataDTO customsData)
        {
            var mappedCustomsData = _mapper.Map<CustomsData>(customsData);

            _unit.CustomsRepository.Update(mappedCustomsData);

            return _unit.SaveAsync();
        }

        public Task DeleteAsync(CustomsDataDTO model)
        {
            var mappedCalculate = _mapper.Map<CustomsData>(model);

            _unit.CustomsRepository.Delete(mappedCalculate);

            return _unit.SaveAsync();
        }
    }
}