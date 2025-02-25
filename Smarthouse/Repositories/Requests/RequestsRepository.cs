using Smarthouse.Models;
using Smarthouse.Models.ViewModels;
using Smarthouse.Repositories.Json;
using Smarthouse.Repositories.Tools;
using Smarthouse.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Smarthouse.Repositories.Requests
{
    public class RequestsRepository : IRequestsRepository
    {
        private readonly string urlString = "http://";
        //  private readonly List<SiteModel> siteModelList = Startup.siteModelList;

        public RequestsRepository()
        {
            //  this.jsonRepository = jsonRepository;
            // to jest dla symulatora
            if (Program.EnviromentType == Program.Enviroment.Simulator) urlString = "http://localhost:55555/Command/";
        }

        public SendRequestReturnModel SendRequests(HashSet<string> urls)
        //służy do wysyłania zapytań do adresów z przesłanej listy
        {
            string responseFromArduino = "";
            string responseString = "";
            List<string> networkIssue = new List<string>();
            foreach (string url in urls)
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        responseString = client.DownloadString(url);
                        Console.WriteLine("Requests sent to: " + url);
                    }
                    catch (WebException)
                    {
                        responseString = null;
                        networkIssue.Add(url);
                    };
                }
                if (responseString != null && responseString != "")
                {
                    responseString = responseString.Replace(Environment.NewLine, string.Empty);
                    responseFromArduino += responseString;
                }
            }

            if (responseFromArduino != null && responseFromArduino != "")
            {
                responseFromArduino = responseFromArduino.Remove(responseFromArduino.Length - 1);
            }
            SendRequestReturnModel sendRequestReturnModel = new SendRequestReturnModel(responseFromArduino, networkIssue);

            return sendRequestReturnModel;
        }


        public SendRequestReturnModel CommandExecution(string site = null, string operation = null)
        {
            HashSet<string> url = UrlBuilder2(site, operation);
            Console.WriteLine("CommandExecution for site: " + site + " ; and operation: " + operation);
            return SendRequests(url);
        }



        public string GetBulkOperationSwitchesForSite(string siteName, string action = null)
        {
            ApplicationSettingsModel ApplicationSettings = StaticData.applicationSettingsModel;
            string finalOperationString = "";

            //Dokumentacja!!!!
            // tutaj skończyłem pisać w nowy sposob
            // opisac jak to jest a Areas
            // co jest standardowe w Areas


            string[] operations = ApplicationSettings.BulkActionsStieParamters[siteName].Split(";");

            foreach (string operation in operations)
            {
                finalOperationString += (operation + action);
            }

            return finalOperationString;
        }





        // to jest do budowania słownika (Mapy klucz=> wartość) składającego się z prametru i wartości 
        // budowane jest z response otrzymanego z Arduino
        public List<SensorValueModel> SensorAndValuesFromArduinoResponse(string responseString)
        {
            List<SensorValueModel> dataFromSensors = new List<SensorValueModel>();
            //    Dictionary<string, string> wynikSlownik = new Dictionary<string, string>();
            Console.WriteLine("Response data = " + responseString);
            if (responseString != null && responseString != "")
            {
                string[] eachPair = responseString.Split(";");
                var substringsEachPair = new HashSet<string>(eachPair);

                foreach (string substringEachElement in substringsEachPair)
                {
                    string[] substringsTwo = substringEachElement.Split("=");
                    string key = substringsTwo[0];
                    string value = substringsTwo[1];
                    Console.WriteLine("Sensor reading: " + key + " = " + value);
                    try
                    {
                        //wynikSlownik.Add(key, value);
                        // działa
                        dataFromSensors.Add(new SensorValueModel(key, value));
                    }
                    catch (ArgumentException argumentException)
                    {
                        Console.WriteLine("Wyjątek: podwója zmienna - " + argumentException);
                        break;
                    }

                }
                // dataFromSensors = wynikSlownik.Select(x => new SensorValueModel(x.Key, x.Value)).ToList();

                // to wyrzej zmienić, po co najpier przypisuje do wynikSłownik zeby zaraz go przeszukiwać i znowu robić listę?
                // tutaj chyba powinno zostać sprawdzone z pliku json co to jest za wskaznik czyli typ - swiatlo
                // mając wskażnik zwrócony, mogę przeszukać plik konfguracyjny a raczej już to co jest zaczytane i przypisać odpowiednią komendę oraz typ 

                Console.WriteLine(dataFromSensors);

                //poniżej jest funkcja która przeszukuje jsona z akcjami arduino
                return dataFromSensors;
            }
            return null;
        }


        public List<OperationsModel> OperationParmForButtons()
        {

            List<OperationsModel> operationsModelList = new List<OperationsModel>();
            // przeniesione na góre
            // List<SiteModel> siteModelList = new List<SiteModel>();
            //siteModelList = jsonRepository.ReadArduinoSettingsFile().ToList();
            //siteModelList = Startup.siteModelList;
            foreach (SiteModel sm in StaticData.siteModelList)
            {
                foreach (OperationsModel op in sm.Operations)
                {
                    // TUTAJ POXNIEJ MOZNA ZMIENIC ZEBY UZWYAL JEDNEGO MODELU DANYCH, TEGO SAMEGO DO WYSYLANIA REQUSTÓW CO ODBIERANIA ODPOWIDZI Z ARDUINO
                    OperationsModel operationsModel = new OperationsModel(op.OperationParmName, op.OperationReturnString, op.OperationType);
                    operationsModelList.Add(operationsModel);
                }
            }
            return operationsModelList;
        }



        public HashSet<string> UrlBuilder(string siteName = null, string operationName = null)
        {
            HashSet<string> urlList = new HashSet<string>();

            foreach (var siteData in StaticData.siteModelList)
            {
                if (siteName != null)
                {
                    //if (siteName == "All")
                    //{
                    //    string url = urlString;
                    //    url += siteData.SiteIP.MainIPAddress + "/" + operationName;
                    //    urlList.Add(url);
                    //}
                    //else
                    if (siteData.SiteName == siteName)
                    {
                        string url = urlString;
                        url += siteData.SiteIP.MainIPAddress + "/" + operationName;
                        urlList.Add(url);
                    }
                }
                else
                {
                    string url = urlString;
                    url += siteData.SiteIP.MainIPAddress + "/";
                    urlList.Add(url);
                }
            }
            return urlList;
        }


        // creates list of address of all microctr with switches if any
        // switchStateList - holds list of switches with command on or off separated with ;
        // if parametrer is not given, empty requests are sent to all microctrl to recieve fresh data of sensor states
        public HashSet<string> UrlBuilder2(string siteName = null, string switchStateList = null)
        {
            HashSet<string> urlList = new HashSet<string>();
            Dictionary<string, string> switch2ipMapping = new Dictionary<string, string>();

            List<string> allSwitches = new List<string>();

            // if switchStateList is empty send refresh request to all microctrl
            //if not empty find analise and assign request to 
            if (switchStateList != null)
            {
             //   switchStateList = switchStateList.Remove(switchStateList.Length - 1);

                string onlySwitch = "";

                if (switchStateList.Contains(';'))
                {
                    allSwitches = switchStateList.Split(";").ToList();
                }
                else {
                    allSwitches.Add(switchStateList);
                }
                      

                foreach (string eachSwitchAndAction in allSwitches)
                {
                    foreach (SiteModel siteData in StaticData.siteModelList)
                    {

                        char lastChar = ToolsAndUtilities.GetLastChar(eachSwitchAndAction);
                        if (lastChar.Equals('f'))
                        {
                            onlySwitch = ToolsAndUtilities.CutLastChars(eachSwitchAndAction, 3);
                        }
                        else
                        {
                            onlySwitch = ToolsAndUtilities.CutLastChars(eachSwitchAndAction, 2);
                        }

                        //Jeżeli na liście jest taki przełącznik zapisz jego IP -> najlpiej w tablicy [switchID, ipaddress]
                        if (siteData.Operations.Any(x => x.OperationParmName == onlySwitch))
                        {
                            switch2ipMapping.Add(eachSwitchAndAction, siteData.SiteIP.MainIPAddress);
                        }
                    }
                }

            }
            // na tym etapie mam wszystkie addresy przełączników, których stan trzeba zmienić wraz z ifnormacją on czy off np.: SGSoff

            //grupuję zbiór po adresach IP jakie są przypisane do przekązników
            Dictionary<string, string> GroupedByIpAdressDictionary = switch2ipMapping.GroupBy(o => o.Value).ToDictionary(g => g.Key, g => string.Join(";", g.Select(x => x.Key).ToArray()));

            foreach (SiteModel siteData in StaticData.siteModelList)
            {
                if (!GroupedByIpAdressDictionary.Any(y => y.Key == siteData.SiteIP.MainIPAddress))
                    GroupedByIpAdressDictionary.Add(siteData.SiteIP.MainIPAddress, "refresh");
            }


            // każdego z adresów IP oraz przpisanych mu przełączników tworzę poszególny adres wywołania akcji na arduino
            foreach (KeyValuePair<string, string> entry in GroupedByIpAdressDictionary)
            {
                
                //string url = "http://" + entry.Key + "/" + entry.Value;
                string url = urlString + entry.Key + "/" + entry.Value;
                urlList.Add(url);
            }
            return urlList;
        }



        public string ReadSingleValue(string siteName, string sensorName)
        {
            HashSet<string> url = UrlBuilder(siteName);
            if (url.Count > 0)
            {
                SendRequestReturnModel response = SendRequests(url);

                ArduinoSensorViewModel model = new ArduinoSensorViewModel
                {
                    SensorValueModelCollection = SensorAndValuesFromArduinoResponse(response.ResponseString)
                };

                foreach (SensorValueModel sensorValueModel in model.SensorValueModelCollection)
                {
                    if (sensorValueModel.SensorName == sensorName) return sensorValueModel.SensorValue;
                }
            }
            return null;
        }


        public string SendSingleValue(string siteName, string operation, string sensorName)
        {
            HashSet<string> url = UrlBuilder(siteName, operation);
            if (url.Count > 0)
            {
                SendRequestReturnModel response = SendRequests(url);

                ArduinoSensorViewModel model = new ArduinoSensorViewModel
                {
                    SensorValueModelCollection = SensorAndValuesFromArduinoResponse(response.ResponseString)
                };

                foreach (SensorValueModel sensorValueModel in model.SensorValueModelCollection)
                {
                    if (sensorValueModel.SensorName == sensorName) return sensorValueModel.SensorValue;
                }
            }
            return null;
        }


    }
}
