using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamManager.Models
{
    /// <summary>
    /// A class representing a database user with diferent privileges: Admin = 1, User = 2, Analyst = 3
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public int Role { get; set; }
    }
}
