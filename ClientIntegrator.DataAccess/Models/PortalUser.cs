using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace ClientIntegrator.DataAccess.Models
{
    public class PortalUser : IdentityUser<long>
    {
        public PortalUser() : base() { }
        public PortalUser(string userName) : base(userName) { }

        public long? OrganizationId { get; set; }
        public bool IsActive { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
