using System;
using System.Collections.Generic;

namespace ClientIntegrator.DataAccess.Models
{
    public partial class ConfigurationDict
    {
        public ConfigurationDict()
        {
            Configurations = new HashSet<Configuration>();
        }

        public long Id { get; set; }
        public string Key { get; set; }
        public string DefaultValue { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public bool IsEncrypted { get; set; }
        public DateTime CreateAt { get; set; }
        public long CreateBy { get; set; }
        public DateTime UpdateAt { get; set; }
        public long UpdateBy { get; set; }

        public virtual ICollection<Configuration> Configurations { get; set; }
    }
}
