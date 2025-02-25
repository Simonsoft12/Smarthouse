using Smarthouse.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// This class is to hold sensors data returned from arduino
namespace Smarthouse.Models.ViewModels
{
    public class SensorValueModel 
    {
        public string SensorName { get; set; }
        public string SensorValue { get; set; }
        public string OperationParmName { get; set; }
        public string OperationType { get; set; }
        public SensorValueModel(){}

        public SensorValueModel(string name, string value)
        {
            SensorName = name;
            SensorValue = value;
            //OperationParmName = ExecPropertySearch(name);
            foreach (SiteModel sm in StaticData.siteModelList)
            {
                // TUTAJ POXNIEJ MOZNA ZMIENIC ZEBY UZWYAL JEDNEGO MODELU DANYCH, TEGO SAMEGO DO WYSYLANIA REQUSTÓW CO ODBIERANIA ODPOWIDZI Z ARDUINO
                foreach (OperationsModel op in sm.Operations)
                {
                    if (op.OperationReturnString == name)
                    {
                        OperationParmName = op.OperationParmName;
                        OperationType = op.OperationType;
                    }
                }
            }
        }
    }
}
