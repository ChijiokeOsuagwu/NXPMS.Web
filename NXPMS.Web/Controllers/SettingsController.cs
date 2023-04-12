using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NXPMS.Base.Services;

namespace NXPMS.Web.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IGlobalSettingsService _globalSettingsService;

        public SettingsController(IGlobalSettingsService globalSettingsService)
        {
            _globalSettingsService = globalSettingsService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}