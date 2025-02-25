using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Smarthouse.Models;
using Smarthouse.Repositories.Requests;

namespace Smarthouse.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private readonly IRequestsRepository requestsRepository;

        // konstruktor do inicjacji interfaców klas pomocniczych
        public HomeController(IRequestsRepository requestsRepository)
        {
            this.requestsRepository = requestsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

      
        //Ajax function
        public IActionResult GetSensorsInformation(string site)
        {
            HashSet<string> url = new HashSet<string>();

          //  url = requestsRepository.UrlBuilder2("All", "SGBoff");

            SendRequestReturnModel response = requestsRepository.CommandExecution(site);
            ArduinoSensorViewModel model = new ArduinoSensorViewModel
            {
                SensorValueModelCollection = requestsRepository.SensorAndValuesFromArduinoResponse(response.ResponseString),
                OperationParmForButtonsCollection = requestsRepository.OperationParmForButtons(),
                NetworkIssueURLs = response.NetworkIssueUrlList
            };

            if (response.NetworkIssueUrlList.Count > 0)
                return Json(new { Model = model, Status = "Error", Message = "Problem z siecią" });
            return Json(new { Model = model, Status = "Success", Message = "Jest OK" });
        }

        //Ajax function for sending command to one site -> returns all sensor values
        public IActionResult SendCommand(string site, string operation)
        {
            SendRequestReturnModel response = requestsRepository.CommandExecution(site, operation);
            ArduinoSensorViewModel model = new ArduinoSensorViewModel
            {
                SensorValueModelCollection = requestsRepository.SensorAndValuesFromArduinoResponse(response.ResponseString),
                OperationParmForButtonsCollection = requestsRepository.OperationParmForButtons(),
                NetworkIssueURLs = response.NetworkIssueUrlList
            };
            if (response.NetworkIssueUrlList.Count > 0)
                return Json(new { Model = model, Status = "Error", Message = "Problem z siecią" });
            else return Json(new { Model = model, Status = "Success", Message = "Jest OK" });
        }

        // ajax function
        public IActionResult LightsOut(string switchList)
        {
            //lista wszystkich operacji dla przełączników konkretnego siteu, pobrana z pliku konfiguracyjnego - nie rozróżnia czy światło czy nie.
            // W tej funkcji powinno się sprawdzać typ przełącznika na podstawie drugiego pliku i wybierac tylko przełączniki dotyczące światła
            // w url builder albo nowa funkcja i powinno się sprawdzac nazwy site i adres IP ich, tak aby wysłać wszystie przełączniki jako jeden string do konkretnego adresu IP

            HashSet<string> url = new HashSet<string>();

            url = requestsRepository.UrlBuilder2("dummy", switchList);

            SendRequestReturnModel response = requestsRepository.SendRequests(url);
            ArduinoSensorViewModel model = new ArduinoSensorViewModel
            {
                NetworkIssueURLs = response.NetworkIssueUrlList,
                OperationParmForButtonsCollection = requestsRepository.OperationParmForButtons(),
                SensorValueModelCollection = requestsRepository.SensorAndValuesFromArduinoResponse(response.ResponseString),
            };
            if (response.NetworkIssueUrlList.Count > 0)
                return Json(new { model, status = "error", message = "problem z siecią" });
            else return Json(new { model, status = "success", message = "jest ok" });
        }

        public IActionResult About()
        {

            ViewData["Message"] = "System obsługi domu inteligentnego";
            return View();
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
