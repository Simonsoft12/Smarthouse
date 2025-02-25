using Applikacja.Models;

namespace Applikacja.Repositories.Json
{
    public interface IJsonRepository
    {
        SettingsModel ReadSettingsFile();
        void SaveSettingsFile(SettingsModel data);
    }
}
