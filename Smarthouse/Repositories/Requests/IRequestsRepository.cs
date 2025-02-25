using Smarthouse.Models;
using Smarthouse.Models.ViewModels;
using System.Collections.Generic;

namespace Smarthouse.Repositories.Requests
{
    // zbiór opearcji do wysyłania zapytań i odbierania odpowiedzi z Arduino
    public interface IRequestsRepository
    {
        // wykorzystywane do wysyłania rządań do Arduino, zwraca string z danymi o stanie czujników
        SendRequestReturnModel SendRequests(HashSet<string> urls);
        // do dzielnia wyników na pary klucz = wartość, czyli w programie nazwa_czujnika = stan_czujnika
        List<SensorValueModel> SensorAndValuesFromArduinoResponse(string responseString);
        HashSet<string> UrlBuilder(string site = null, string operation = null);
        HashSet<string> UrlBuilder2(string siteName = null, string switchStateList = null);
        List<OperationsModel> OperationParmForButtons();
        string ReadSingleValue(string siteName, string sensorName);
        string SendSingleValue(string siteName, string operation, string sensorName);
        SendRequestReturnModel CommandExecution(string site = null, string operation = null);
        /// <summary>
        /// Wczytuje parametry operacji dla konkretnego miejsca.
        /// </summary>
        string GetBulkOperationSwitchesForSite(string siteName, string action = null);
    }
}
