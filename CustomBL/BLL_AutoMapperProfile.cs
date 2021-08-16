using AutoMapper;
using Custom.BL.Models;
using Custom.DAL.Entities;

namespace Custom.BL
{
    public class BLL_AutoMapperProfile : Profile
    {
        public BLL_AutoMapperProfile()
        {
            CreateMap<CustomsData, CustomsDataDTO>().ReverseMap();
        }
    }
}