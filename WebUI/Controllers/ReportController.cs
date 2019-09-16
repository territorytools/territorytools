using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using AlbaClient;
using AlbaClient.AlbaServer;
using AlbaClient.Controllers.AlbaServer;
using AlbaClient.Controllers.UseCases;
using AlbaClient.Models;
using Microsoft.AspNetCore.Authorization;
using cuc = Controllers.UseCases;
using Controllers.UseCases;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using io = System.IO;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace WebUI.Controllers
{
    [Authorize]
    public class ReportController : AuthorizedController
    {
        public ReportController(
            IStringLocalizer<ReportController> localizer,
            IAlbaCredentials credentials,
            WebUI.Services.IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor) : base(
                localizer,
                credentials,
                authorizationService,
                optionsAccessor)
        {
        }

        [Authorize]
        public IActionResult Index()
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            return View();
        }

        [Authorize]
        public IActionResult SummarizeCompleted()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var assignments = GetAllAssignments(account, user, password)
                    .Where(a => a.LastCompleted >= new DateTime(2019,9,1) )
                    .ToList();

                var report = new SummarizeCompletedReport()
                {
                    CompletedCount = assignments.Count
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
