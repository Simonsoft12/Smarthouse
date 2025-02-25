using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Smarthouse.Models;
using Smarthouse.Repositories.Json;
using Smarthouse.Repositories.Requests;
using Smarthouse.Static;

namespace Smarthouse.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        List<SiteModel> siteModelList = StaticData.siteModelList;
        ApplicationSettingsModel applicationSettingsModel = StaticData.applicationSettingsModel;

        private IRequestsRepository requestsRepository;
        private IJsonRepository jsonRepository;

        public AdminController(
            IRequestsRepository requestsRepository, IJsonRepository jsonRepository)
        {
            this.requestsRepository = requestsRepository;
            this.jsonRepository = jsonRepository;
        }

        public IActionResult Index()
        {
           

            return View();
        }

        //
        // po co tutaj zapisuje komendy, skoro wczesniej wczytuje je w readarduinosettings ( OperationParmName i OperationReturnString )
        //Ajax function
        public IActionResult SaveConfiguration()
        {

            ApplicationSettingsModel applicationSettingsModel = new ApplicationSettingsModel();


            applicationSettingsModel.BulkActionsStieParamters = new Dictionary<string, string>();

            applicationSettingsModel.BulkActionsStieParamters.Add("Salon", "SG;SJ;SPtv;SPsofa;TG");



            jsonRepository.SaveApplicationSettingsFile(applicationSettingsModel);

            var model = new 
            {
                
            };

            return Json(new { Model = model, Status = "Success", Message = "Jest OK" });
        }

    }
}