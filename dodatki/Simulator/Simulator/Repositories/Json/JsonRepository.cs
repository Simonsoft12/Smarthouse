using Applikacja.Models;
using Newtonsoft.Json;
using System.IO;

namespace Applikacja.Repositories.Json
{
    public class JsonRepository : IJsonRepository
    {
        public SettingsModel ReadSettingsFile()
        {
            SettingsModel data = new SettingsModel();
            using (StreamReader r = new StreamReader("C:\\Smarthouse\\arduino_settings.json"))
            {
                string jsonData = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<SettingsModel>(jsonData);
            }
            return data;
        }


        public void SaveSettingsFile(SettingsModel data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(@"C:\\Smarthouse\\arduino_settings.json", json);
        }
    }
}
