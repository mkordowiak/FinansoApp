using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinansoData.Models;

namespace FinansoData.Repository.Group
{
    public interface IGroupCrudRepository
    {
        bool Add(FinansoData.Models.Group group);
        bool Update(FinansoData.Models.Group group);
        bool Delete(FinansoData.Models.Group group);
        bool Save();
        Task<bool> SaveAsync();
        Task<bool> UpdateAsync(FinansoData.Models.Group group);
    }
}
