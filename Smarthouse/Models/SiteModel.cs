using Smarthouse.Models;
using System.Collections.Generic;

namespace Smarthouse.Models
{
    public class SiteModel
    {
        public string SiteName { get; set; }
        public SiteIPModel SiteIP { get; set; }
        public ICollection<OperationsModel> Operations { get; set; }
    }
}
