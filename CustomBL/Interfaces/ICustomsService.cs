using Custom.BL.Interfaces;
using Custom.BL.Models;

namespace Custom.BL.Services
{
    public interface ICustomsService : ICrud<CustomsDataDTO>
    {
        int GetResult(CustomsDataDTO model);
    }
}