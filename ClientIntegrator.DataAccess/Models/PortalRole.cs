using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.DataAccess.Models
{
    public class PortalRole : IdentityRole<long>
    {
        public PortalRole() : base() { }
        public PortalRole(string roleName) : base(roleName) { }
    }
}
