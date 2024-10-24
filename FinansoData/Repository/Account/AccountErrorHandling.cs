using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Repository.Account
{
    public class AccountErrorHandling : IAccountErrorHandling
    {
        public bool DatabaseError { get; set; }
        public bool EmailAlreadyExists { get; set; }
        public bool RegisterError { get; set; }
        public bool AssignUserRoleError { get; set; }
        public bool UserNotFound { get; set; }
        public bool WrongPassword { get; set; }
    }
}
