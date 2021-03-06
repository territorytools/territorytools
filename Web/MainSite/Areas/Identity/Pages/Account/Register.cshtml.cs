﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TerritoryTools.Alba.Controllers.Models;
using TerritoryTools.Web.Data;

namespace TerritoryTools.Web.MainSite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly MainDbContext _database;
        private readonly List<string> _users;
        private readonly List<string> _adminUsers;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            MainDbContext database,
             IOptions<WebUIOptions> optionsAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _database = database;
            _users = optionsAccessor
                .Value
                .Users
                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            _adminUsers = optionsAccessor
                .Value
                .AdminUsers
                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var territoryUser = _database.TerritoryUser
                    .FirstOrDefault(u => 
                        u.Email != null && 
                        Input.Email != null && 
                        string.Equals(u.Email.ToUpper(), Input.Email.ToUpper()));

                bool existsInConfigurationFileAsUser = false;
                bool existsInConfigurationFileAsAdmin = false;

                if (territoryUser == null)
                {
                    existsInConfigurationFileAsUser = _users.Exists(u =>
                        Input.Email != null &&
                        string.Equals(u.ToUpper(), Input.Email.ToUpper()));

                    existsInConfigurationFileAsAdmin = _adminUsers.Exists(u =>
                        Input.Email != null &&
                        string.Equals(u.ToUpper(), Input.Email.ToUpper()));
                }

                if(existsInConfigurationFileAsUser || existsInConfigurationFileAsAdmin)
                {
                    territoryUser = new Entities.TerritoryUser
                    {
                        Created = DateTime.Now,
                        Email = Input.Email,
                        Role = existsInConfigurationFileAsAdmin ? "Administrator" : "User"
                    };

                    _database.TerritoryUser.Add(territoryUser);
                    _database.SaveChanges();
                }

                if (territoryUser == null)
                {
                    ModelState.AddModelError(string.Empty, "That email is not in our system.  You must be invited.");
                    return Page();
                }

                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                territoryUser.AspNetUserId = user.Id;
                _database.Update(territoryUser);
                _database.SaveChanges();

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
