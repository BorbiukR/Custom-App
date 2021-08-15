using Custom.BL.Interfaces;
using Custom.BL.Models;
using System.Threading.Tasks;

namespace Custom.BL.Services
{
    public interface ICustomsService : ICrud<CustomsDataDTO>
    {
        Task<int> GetResult(CustomsDataDTO model);
    }
}