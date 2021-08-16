using AutoMapper;
using Custom.Api.Models.Request;
using Custom.BL.Models;

namespace Custom.Api
{
    public class API_AutoMapperProfile : Profile
    {
        public API_AutoMapperProfile()
        {
            CreateMap<CustomsBusRequest, CustomsDataDTO>().ReverseMap();
            CreateMap<CustomsBikeRequest, CustomsDataDTO>().ReverseMap();
            CreateMap<CustomsCarRequest, CustomsDataDTO>().ReverseMap();
            CreateMap<CustomsElectricCarRequest, CustomsDataDTO>().ReverseMap();
            CreateMap<CustomsTruckRequest, CustomsDataDTO>().ReverseMap();
        }     
    }
}