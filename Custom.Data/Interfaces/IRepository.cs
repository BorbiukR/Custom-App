using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Custom.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<T> GetById(int id);
        IEnumerable<T> FindByCondition(Func<T, bool> predicate);
        Task AddAsync(T value);
        void Update(T value);
        void Delete(T item);
    }
}