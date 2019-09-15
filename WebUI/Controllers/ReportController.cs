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
    public class ReportController : Controller
    {
        string account;
        string user;
        string password;
        WebUI.Services.IAuthorizationService authorizationService;

        public ReportController(
            IStringLocalizer<ReportController> localizer,
            IAlbaCredentials credentials,
            WebUI.Services.IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            this.localizer = localizer;
            account = credentials.Account;
            user = credentials.User;
            password = credentials.Password;
            this.authorizationService = authorizationService;
            options = optionsAccessor.Value;
        }

        readonly WebUIOptions options;

        public IActionResult Index()
        {
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
