using System;
using System.Collections.Generic;

namespace ClientIntegrator.DataAccess.Models
{
    public class Organization
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool ShowUsersAllFiles{ get; set; }
        public DateTime CreateAt { get; set; }
        public long CreateBy { get; set; }
        public DateTime UpdateAt { get; set; }
        public long UpdateBy { get; set; }
        public virtual ICollection<PortalUser> PortalUsers { get; set; }
        public virtual ICollection<Configuration> Configurations { get; set; }
    }
}
