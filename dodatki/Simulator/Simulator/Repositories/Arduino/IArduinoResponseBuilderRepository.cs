using Applikacja.Models;

namespace Applikacja.Repositories.Arduino
{
    public interface IArduinoResponseBuilderRepository
    {
        // !!!!! trzeba zawsze dodać interface do Startup.cs !!!!!
        // tworzy słownik dla akcji dla wszystkich Arduino, który używany jest w View.

        void LoadSettings(SettingsModel settings);
        string GenerateResponseString(string IP, string parm);
        void SesnorSimulatorEngine(string IP, string parameters);
        string SerializeResponse();
    }
}
