using Custom.DAL.Entities;
using System;

namespace Custom.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<Calculate> Calculates { get; }
        public void Save();
    }
}