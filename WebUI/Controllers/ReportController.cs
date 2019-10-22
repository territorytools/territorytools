using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WebUI.Controllers
{
    [Authorize]
    public class ReportController : AuthorizedController
    {
        public ReportController(
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            Services.IAuthorizationService authorizationService,
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
        public IActionResult ByPublisher()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var groups = GetAllAssignments(account, user, password)
                    .Where(a => !string.IsNullOrWhiteSpace(a.SignedOutTo))
                    .GroupBy(a => a.SignedOutTo)
                    .ToList();

                var publishers = new List<Publisher>();
                foreach (var group in groups.OrderBy(g => g.Key))
                {
                    var pub = new Publisher() { Name = group.Key };
                    var ordered = group.OrderByDescending(a => a.SignedOut);
                    foreach (var item in ordered)
                    {
                        pub.Territories.Add(item);
                    }

                    publishers.Add(pub);
                }

                return View(publishers);
            }
            catch (Exception)
            {
                throw;
            }
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

                var allAssignments = GetAllAssignments(account, user, password)
                    .ToList();

                var now = DateTime.Now;
                int thisYear = now.Month >= 9 ? now.Year : (now.Year - 1);

                var report = new SummarizeCompletedReport();

                for (int i = 0; i <= 10; i++)
                {
                    int year = thisYear - i;
                    report.SummaryItems.Add(
                        new SummaryItem
                        {
                            Name = $"{year}-{year + 1}",
                            Value = allAssignments
                                .Where(a => a.LastCompleted >= new DateTime(year, 9, 1)
                                && a.LastCompleted < new DateTime(year + 1, 9, 1))
                                .ToList()
                                .Count
                                .ToString()
                        });
                }

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult NeverCompleted()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var publishers = GetUsers(account, user, password)
                    .OrderBy(u => u.Name)
                    .ToList();

                var assignments = GetAllAssignments(account, user, password)
                    // Territories never worked
                    .Where(a => a.LastCompleted == null && a.SignedOut == null)
                    .OrderBy(a => a.Description)
                    .ThenBy(a => a.Number)
                    .ToList();

                var report = new NeverCompletedReport()
                {
                    Publishers = publishers,
                    Assignments = assignments
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult Available()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var users = GetUsers(account, user, password)
                    .OrderBy(u => u.Name)
                    .ToList();

                var assignments = GetAllAssignments(account, user, password)
                    .Where(a => string.Equals(
                        a.Status,
                        "Available",
                        StringComparison.OrdinalIgnoreCase))
                    .OrderBy(a => a.LastCompleted)
                    .ThenBy(a => a.Number)
                    .ToList();

                var report = new NeverCompletedReport()
                {
                    // TODO: Rename Publishers to Users
                    Publishers = users,
                    Assignments = assignments
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult AlbaUsers()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var users = GetAlbaUsers()
                    .OrderBy(u => u.Name)
                    .ToList();

                return View(users);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
