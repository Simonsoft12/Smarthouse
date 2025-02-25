using Smarthouse.Models;
using Smarthouse.Repositories.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smarthouse.Static
{
    public static class StaticData
    {
        public static List<SiteModel> siteModelList = new List<SiteModel>();
        public static ApplicationSettingsModel applicationSettingsModel = new ApplicationSettingsModel();
       
        public static void LoadData() {
            IJsonRepository jsonRepository = new JsonRepository();
            siteModelList = jsonRepository.ReadArduinoSettingsFile().ToList();
            applicationSettingsModel = jsonRepository.ReadApplicationSettingsFile();
        }
    }
}
