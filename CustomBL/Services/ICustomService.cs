using Custom.BL.Models;

namespace Custom.BL.Services
{
    public interface ICustomService
    {
        int GetResult(CalculateModel model);
    }
}