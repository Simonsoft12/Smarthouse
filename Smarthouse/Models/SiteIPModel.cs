using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smarthouse.Models
{
    public class SiteIPModel
    {
        public string MainIPAddress { get; set; }
        public List<string> AdditionalIPAddresses { get; set; }
    }
}
