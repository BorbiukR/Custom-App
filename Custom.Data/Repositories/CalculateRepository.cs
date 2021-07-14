using Custom.DAL.Entities;
using Custom.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models;

namespace Custom.DAL.Repositories
{
    public class CalculateRepository : IRepository<Calculate>
    {
        private readonly AppDBContext _db;
        
        public CalculateRepository(AppDBContext context)
        {
            _db = context;
        }

        public IEnumerable<Calculate> GetAll()
        {
            return _db.Calculates;
        }

        public Calculate Get(int id)
        {
            return _db.Calculates.Find(id);
        }

        public void Create(Calculate calculate)
        {
            _db.Calculates.Add(calculate);
        }

        public void Update(Calculate calculate)
        {
            _db.Entry(calculate).State = EntityState.Modified;
        }

        public IEnumerable<Calculate> Find(Func<Calculate, bool> predicate)
        {
            return _db.Calculates.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            var calculate = _db.Calculates.Find(id);
            
            if (calculate != null)
                _db.Calculates.Remove(calculate);
        }
    }
}