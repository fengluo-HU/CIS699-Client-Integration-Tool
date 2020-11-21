using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.Common.Models
{
    public class BaseModel
    {
        public long Id { get; set; }
        public string CreateBy { get; set; }
        public string CreateAt { get; set; }
        public string UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}
