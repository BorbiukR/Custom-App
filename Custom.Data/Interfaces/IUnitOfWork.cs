using System.Threading.Tasks;

namespace Custom.DAL.Interfaces
{
    public interface IUnitOfWork 
    {
        ICustomsRepository CustomsRepository { get; }
        Task<int> SaveAsync();
    }
}