using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository.Account
{
    public interface IAccountErrorHandling
    {
        bool DatabaseError { get; set; }
        bool EmailAlreadyExists { get; set; }
        bool RegisterError { get; set; }
        bool AssignUserRoleError { get; set; }
        public bool UserNotFound { get; set; }
        bool WrongPassword { get; set; }
    }
}
