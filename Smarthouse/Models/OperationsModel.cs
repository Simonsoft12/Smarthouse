using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smarthouse.Models
{
    public class OperationsModel
    {
        public OperationsModel(){}

        public OperationsModel(string operationParmName, string operationReturnString, string operationType)
        {
            OperationParmName = operationParmName;
            OperationReturnString = operationReturnString;
            OperationType = operationType;
        }

        public string OperationParmName { get; set; }
        public string OperationReturnString { get; set; }
        public string OperationType { get; set; }
    }
}
