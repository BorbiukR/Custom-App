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

        public int GetResult(CustomsDataDTO customsData)
        {
            if (customsData == null)
                throw new CustomsException("Customs Data is null");

            return customsData.VehicleType switch
            {
                VehicleType.Car =>
                    GetCarCustomValue(customsData.FuelType, customsData.EngineVolume, customsData.Price, customsData.Year),
                VehicleType.ElectricCar => 
                    GetElectricCarCustomValue(customsData.FuelType, customsData.EngineVolume),
                VehicleType.Truck => 
                    GetTruckCustomValue(customsData.Price, customsData.Year, customsData.EngineVolume, customsData.VehicleWeight),
                VehicleType.Bus => 
                    GetBusCustomValue(customsData.Price, customsData.Year, customsData.EngineVolume, customsData.FuelType),
                VehicleType.Bike => 
                    GetBikeCustomValue(customsData.Price, customsData.Year, customsData.EngineVolume),
                _ => 
                    throw new NotImplementedException()
            };      
        }

        private int GetCarCustomValue(FuelType fuelType, int engineVolume, int price, DateTime year)
        {
            if (engineVolume < 10 || engineVolume > 30_000)
                throw new CustomsException("Engine volume is not valid");

            if (price < 50 || price > 100_000_000)
                throw new CustomsException("Price is not valid");

            if (year > DateTime.Now || year < new DateTime(1850))
                throw new CustomsException("Year is not valid");

            var importDuty = GetImportDuty(price);
            var exciseValue = GetCarExciseValue(year, fuelType, engineVolume);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private int GetElectricCarCustomValue(FuelType fuelType, int engineVolume)
        {
            if (engineVolume < 10 || engineVolume > 30_000)
                throw new CustomsException("Engine volume is not valid");

            if (fuelType != FuelType.Electric)
                throw new CustomsException("Fuel Type can not be another than Electric");

            return engineVolume;
        }

        private int GetTruckCustomValue(int price, DateTime year, int engineVolume, int fullWeight)
        {
            if (engineVolume < 10 || engineVolume > 30_000)
                throw new CustomsException("Engine volume is not valid");

            if (price < 50 || price > 100_000_000)
                throw new CustomsException("Price is not valid");

            if (year > DateTime.Now || year < new DateTime(1850))
                throw new CustomsException("Year is not valid");

            if (fullWeight > 100_000 || fullWeight < 10)
                throw new CustomsException("Vehicle weight is not valid");

            var importDuty = GetImportDuty(price);
            var exciseValue = GetTruckExciseValue(year, fullWeight, engineVolume);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private int GetBikeCustomValue(int price, DateTime year, int engineVolume)
        {
            if (engineVolume < 10 || engineVolume > 30_000)
                throw new CustomsException("Engine volume is not valid");

            if (price < 50 || price > 100_000_000)
                throw new CustomsException("Price is not valid");

            if (year > DateTime.Now || year < new DateTime(1850))
                throw new CustomsException("Year is not valid");

            var importDuty = GetImportDuty(price);
            var exciseValue = GetBikeExciseValue(year, engineVolume);
            var vat = GetVat(price, importDuty, exciseValue);
            var fullPayment = GetFullPayment(exciseValue, importDuty, vat);

            return fullPayment;
        }

        private int GetBusCustomValue(int price, DateTime year, int engineVolume, FuelType fuelType)
        {
            if (engineVolume < 10 || engineVolume > 30_000)
                throw new CustomsException("Engine volume is not valid");

            if (price < 50 || price > 100_000_000)
                throw new CustomsException("Price is not valid");

            if (year > DateTime.Now || year < new DateTime(1850))
                throw new CustomsException("Year is not valid");

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

        public Task AddAsync(CustomsDataDTO customsData)
        {
            var mappedCustomsData = _mapper.Map<CustomsData>(customsData);

            _unit.CustomsRepository.AddAsync(mappedCustomsData);

            return _unit.SaveAsync();
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