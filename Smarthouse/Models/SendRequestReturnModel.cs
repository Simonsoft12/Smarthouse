using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smarthouse.Models
{
    // służy do przechowywania danych zwróconych z Arduino + listę problematycznyc URL z ktorymi nie mógł się połączyć
    public class SendRequestReturnModel
    {
        public List<String> NetworkIssueUrlList { get; set; }
        public string ResponseString { get; set; }


        public SendRequestReturnModel (string response, List<string> problematicURL)
        {
            this.ResponseString = response;
            this.NetworkIssueUrlList = problematicURL;
        }

    }
}
