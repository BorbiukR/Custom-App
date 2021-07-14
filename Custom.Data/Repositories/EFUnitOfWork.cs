using Custom.DAL.Entities;
using Custom.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using Web.Models;

namespace Custom.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _db;
        private CalculateRepository _calculateRepository;

        public EFUnitOfWork(DbContextOptions<AppDBContext> options)
        {
            _db = new AppDBContext(options);
        }

        public IRepository<Calculate> Calculates
        {
            get
            {
                if (_calculateRepository == null)
                    _calculateRepository = new CalculateRepository(_db);

                return _calculateRepository;
            }
        }

        public async void Save()
        {
            await _db.SaveChangesAsync();
        }

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}