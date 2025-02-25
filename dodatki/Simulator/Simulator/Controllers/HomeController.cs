using System.Diagnostics;
using Applikacja.Models;
using Applikacja.Repositories.Arduino;
using Applikacja.Repositories.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Simulator.Models;

namespace Simulator.Controllers
{
    public class HomeController : Controller
    {
        private IArduinoResponseBuilderRepository arduinoResponseBuilderRepository;
        private IJsonRepository jsonRepository;
        // private ILogger<HomeController> _logger;


        public HomeController(IArduinoResponseBuilderRepository arduinoResponseBuilderRepository, IJsonRepository jsonRepository/*, ILogger<HomeController> logger*/)
        {
            this.arduinoResponseBuilderRepository = arduinoResponseBuilderRepository;
            this.jsonRepository = jsonRepository;
            // _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        [Route("Command/{ip}")]
        public IActionResult Command(string ip)
        {
            SettingsModel settings = jsonRepository.ReadSettingsFile();
            arduinoResponseBuilderRepository.LoadSettings(settings);
            string returnString = "";
            Log.Information("Parameters: IP = " + ip);
            if (ip != null)
            {
                returnString = arduinoResponseBuilderRepository.GenerateResponseString(ip, null);
            }
            ViewData["ReturnString"] = returnString;
            Log.Information("ReturnString: " + returnString);
            jsonRepository.SaveSettingsFile(settings);
            return View("Index");
        }

        [Route("Command/{ip}/{parm}")]
        public IActionResult Command(string ip, string parm)
        {
            ArduinoResponseBuilderRepository arduinoResponseBuilderRepository = new ArduinoResponseBuilderRepository(jsonRepository);
            arduinoResponseBuilderRepository.SesnorSimulatorEngine(ip, parm);

            ViewData["ReturnString"] = arduinoResponseBuilderRepository.SerializeResponse();

            return View("Index");
        }



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
