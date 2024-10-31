using FinansoData.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository.Group
{
    public class GroupCrudRepository : IGroupCrudRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupCrudRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Models.Group group)
        {
            try
            {
                _context.Add(group);
                return Save();
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Models.Group group)
        {
            try
            {
                _context.Remove(group);
                return Save();
            }
            catch
            {
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                int saved = _context.SaveChanges();
                return saved > 0 ? true : false;

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                int saved = await _context.SaveChangesAsync();
                return saved > 0 ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Models.Group group)
        {
            try
            {
                _context.Update(group);
                return Save();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Models.Group group)
        {
            try
            {
                _context.Update(group);
                return await SaveAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}
