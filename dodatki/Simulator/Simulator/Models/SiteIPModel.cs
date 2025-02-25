using System.Collections.Generic;

namespace Simulator.Models
{
    public class SiteIPModel
    {
        public string MainIPAddress { get; set; }
        public List<string> AdditionalIPAddresses { get; set; }
    }
}
