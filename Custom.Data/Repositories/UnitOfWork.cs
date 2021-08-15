using Custom.DAL.Interfaces;
using System.Threading.Tasks;
using Web.Models;

namespace Custom.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _customsDb;
        private ICustomsRepository _customsRepository;

        public UnitOfWork(AppDBContext options)
        {
            _customsDb = options;
        }

        public ICustomsRepository CustomsRepository
        {
            get
            {
                if (_customsRepository == null)
                    _customsRepository = new CustomsRepository(_customsDb);

                return _customsRepository;
            }
        }

        public async Task<int> SaveAsync() => await _customsDb.SaveChangesAsync();
    }
}