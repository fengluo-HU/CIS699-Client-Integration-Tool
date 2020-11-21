using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.DataAccess.Models
{
    public class PortalUserToken : IdentityUserToken<long>
    {
        public PortalUserToken() : base() { }
    }
}
