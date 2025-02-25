using Smarthouse.Models.ViewModels;
using System.Collections.Generic;

namespace Smarthouse.Models
{

    public class ArduinoSensorViewModel 
    {
        public ICollection<SensorValueModel> SensorValueModelCollection { get; set; }

        public ICollection<OperationsModel> OperationParmForButtonsCollection { get; set; }

        public ICollection <string> NetworkIssueURLs { get; set; }
    }
}
