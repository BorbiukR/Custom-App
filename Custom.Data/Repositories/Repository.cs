using Custom.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Custom.DAL.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected internal readonly AppDBContext _customsDb;

        public Repository(AppDBContext appDBContext)
        {
            _customsDb = appDBContext;
        }

        public async Task AddAsync(T entity) => await _customsDb.Set<T>().AddAsync(entity);

        public void Update(T entity) => _customsDb.Set<T>().Update(entity);

        public void Delete(T item) => _customsDb.Set<T>().Remove(item);

        public IEnumerable<T> FindByCondition(Func<T, bool> expression) =>
            _customsDb.Set<T>().Where(expression);

        public IEnumerable<T> GetAll() => _customsDb.Set<T>().ToList();

        public async Task<T> GetById(int id) => await _customsDb.Set<T>().FindAsync(id);
    }
}