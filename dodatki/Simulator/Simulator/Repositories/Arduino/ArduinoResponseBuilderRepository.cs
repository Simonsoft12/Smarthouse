using Applikacja.Models;
using Applikacja.Repositories.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applikacja.Repositories.Arduino
{
    public class ArduinoResponseBuilderRepository : IArduinoResponseBuilderRepository
    {
        private SettingsModel _settings;
        SettingsModel settingsModel;
        private IJsonRepository jsonRepository;

        public ArduinoResponseBuilderRepository(IJsonRepository jsonRepository)
        {
            this.jsonRepository = jsonRepository;
            this.settingsModel = jsonRepository.ReadSettingsFile();
        }


        public void LoadSettings(SettingsModel settings)
        {
            _settings = settings;
        }

        // tutaj piszę metodę która będzie zapisywać dane parametrów do modelu

        public void SesnorSimulatorEngine(string IP, string parameters)
        {

            string[] singleParameter = parameters.Split(";");

            foreach (string parm in singleParameter)
            {
                for (int counter = 0; counter < settingsModel.Site.Count; counter++)
                {
                    if (settingsModel.Site[counter].SiteIP.MainIPAddress == IP)
                    {
                        for (int counter2 = 0; counter2 < settingsModel.Site[counter].Operations.Count; counter2++)
                        {
                            if (parm == (settingsModel.Site[counter].Operations[counter2].OperationParmName + "on"))
                            {
                                settingsModel.Site[counter].Operations[counter2].OperationReturnStringValue = "1";
                            }
                            else if (parm == (settingsModel.Site[counter].Operations[counter2].OperationParmName + "off"))
                            {
                                settingsModel.Site[counter].Operations[counter2].OperationReturnStringValue = "0";
                            }
                            else
                            {
                                if (settingsModel.Site[counter].Operations[counter2].OperationReturnString.EndsWith("T"))
                                {
                                    Random rnd = new Random();
                                    int temperature = rnd.Next(18, 25);
                                    settingsModel.Site[counter].Operations[counter2].OperationReturnStringValue = temperature.ToString();
                                }
                            }
                        }
                    }
                }
                jsonRepository.SaveSettingsFile(settingsModel);

            }
        }


        public string GenerateResponseString(string IP, string parm)
        {

            string returnedString = "";

            foreach (var site in _settings.Site)
            {
                if (site.SiteIP.MainIPAddress == IP)
                {
                    foreach (var operation in site.Operations)
                    {
                        if (parm == (operation.OperationParmName + "on"))
                        {
                            String value = operation.OperationReturnStringValue = "1";
                            returnedString += operation.OperationReturnString + "=" + operation.OperationReturnStringValue + ";";
                        }
                        else if (parm == (operation.OperationParmName + "off"))
                        {
                            String value = operation.OperationReturnStringValue = "0";
                            returnedString += operation.OperationReturnString + "=" + operation.OperationReturnStringValue + ";";
                        }
                        else
                        {
                            if (operation.OperationReturnString.EndsWith("T"))
                            {
                                Random rnd = new Random();
                                int temperature = rnd.Next(18, 25);
                                operation.OperationReturnStringValue = temperature.ToString();
                            }
                            returnedString += operation.OperationReturnString + "=" + operation.OperationReturnStringValue + ";";
                        }
                    }
                }
            }
            //   returnedString = returnedString.Remove(returnedString.Length - 1);
            return returnedString;
        }


        public string SerializeResponse()
        {
            HashSet<string> returnValuesHS = new HashSet<string>();
            foreach (var site in settingsModel.Site)
            {
                foreach (var operation in site.Operations)
                {
                    string singlePair = operation.OperationReturnString + "=" + operation.OperationReturnStringValue + ";";
                    returnValuesHS.Add(singlePair);
                }
            }
            return string.Join("", returnValuesHS);
        }
    }
}
