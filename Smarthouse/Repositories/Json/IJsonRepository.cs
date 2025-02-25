using Smarthouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smarthouse.Repositories.Json
{
    public interface IJsonRepository
    {
        List<SiteModel> ReadArduinoSettingsFile();
        ApplicationSettingsModel ReadApplicationSettingsFile();
        void SaveApplicationSettingsFile(ApplicationSettingsModel data);
    }
}

