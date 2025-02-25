using Smarthouse.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Smarthouse.Repositories.Json
{
    public class JsonRepository : IJsonRepository
    {
        public List<SiteModel> ReadArduinoSettingsFile()
        {
            List<SiteModel> data = new List<SiteModel>();
            using (StreamReader r = new StreamReader("C:\\Smarthouse\\arduino_settings.json"))
            {
                string jsonData = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<SettingsModel>(jsonData).Site;
            }
            return data;
        }

        public ApplicationSettingsModel ReadApplicationSettingsFile()
        {
            ApplicationSettingsModel data = new ApplicationSettingsModel();
            
            return data;
        }

        public void SaveApplicationSettingsFile(ApplicationSettingsModel data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText("C:\\Smarthouse\\application_settings.json", json);
        }
    }
}

