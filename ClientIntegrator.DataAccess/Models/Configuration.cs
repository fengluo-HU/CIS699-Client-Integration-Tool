using System;
using System.Collections.Generic;

namespace ClientIntegrator.DataAccess.Models
{
    public partial class Configuration
    {
        public long Id { get; set; }
        public long ConfigurationDictId { get; set; }
        public long OrganizationId { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public DateTime CreateAt { get; set; }
        public long CreateBy { get; set; }
        public DateTime UpdateAt { get; set; }
        public long UpdateBy { get; set; }

        public virtual ConfigurationDict ConfigurationDict { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
