using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace TerritoryTools.Web.MainSite.Controllers
{
    public class ScheduleController : Controller
    {
        [Route("/tick")]
        public IActionResult Tick()
        {
            return Ok();
        }
    }
}