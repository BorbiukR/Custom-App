using Custom.DAL.Entities;
using Custom.DAL.Interfaces;
using Web.Models;

namespace Custom.DAL.Repositories
{
    public class CustomsRepository : Repository<CustomsData>, ICustomsRepository
    {
        public CustomsRepository(AppDBContext DbContext) : base(DbContext) { }
    }
}