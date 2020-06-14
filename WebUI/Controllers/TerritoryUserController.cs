using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class TerritoryUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Invitation()
        {
            try
            {
                var invitation = new TerritoryUserInvitation
                {
                    Email = User.Identity.Name
                };

                //if (!IsUser())
                //{
                //    return View(publisher);
                //}

                //var users = GetUsers(account, user, password);
                //var me = users.FirstOrDefault(u => string.Equals(u.Email, User.Identity.Name, StringComparison.OrdinalIgnoreCase));

                //if (me == null)
                //{
                //    return NotFound();
                //}

                //string myName = me.Name;

                //var assignments = GetAllAssignments()
                //    .Where(a => string.Equals(a.SignedOutTo, myName, StringComparison.OrdinalIgnoreCase))
                //    .ToList();

                //publisher.Name = myName;

                //foreach (var item in assignments.OrderByDescending(a => a.SignedOut))
                //{
                //    publisher.Territories.Add(item);
                //}

                //var qrCodeHits = qrCodeActivityService.QRCodeHitsForUser(
                //    publisher.Email);

                //foreach (var hit in qrCodeHits)
                //{
                //    publisher.QRCodeActivity.Add(
                //        new QRCodeHit
                //        {
                //            Id = hit.Id,
                //            ShortUrl = hit.ShortUrl,
                //            OriginalUrl = hit.OriginalUrl,
                //            Created = hit.Created.ToString("yyyy-MM-dd HH:mm:ss"),
                //            HitCount = hit.HitCount.ToString(),
                //            LastIPAddress = hit.LastIPAddress,
                //            LastTimeStamp = hit.LastTimeStamp?.ToString("yyyy-MM-dd HH:mm:ss"),
                //            Subject = hit.Subject,
                //            Note = hit.Note
                //        });
                //}

                return View(null);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
