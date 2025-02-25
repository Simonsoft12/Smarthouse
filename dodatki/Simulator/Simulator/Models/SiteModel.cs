using Simulator.Models;
using System.Collections.Generic;

namespace Applikacja.Models
{
    public class SiteModel
    {
        public string SiteName { get; set; }
        public SiteIPModel SiteIP { get; set; }
        public List<OperationModel> Operations { get; set; }
    }
}
