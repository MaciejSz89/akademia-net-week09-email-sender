using EmailSenderAspNetMvc.Models.Domains;
using EmailSenderAspNetMvc.Models.Repositories;
using EmailSenderAspNetMvc.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmailSenderAspNetMvc.Controllers
{
    public class ConfigurationController : Controller
    {
        EmailConfigurationRepository _emailConfigurationRepository = new EmailConfigurationRepository();
        public ActionResult Configuration(int id = 0)
        {
            var userId = User.Identity.GetUserId();

            var configuration = id == 0 ? GetNewConfiguration(userId) :
                                          _emailConfigurationRepository.GetEmailConfiguration(id, userId);

            var vm = PrepareEditEmailConfigurationViewModel(configuration);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Configuration(EmailConfiguration emailConfiguration)
        {
            var userId = User.Identity.GetUserId();
            emailConfiguration.UserId = userId;
            emailConfiguration.EmailAddress.UserId = userId;

            if (!ModelState.IsValid)
            {
                var vm = PrepareEditEmailConfigurationViewModel(emailConfiguration);
                return View("Configuration", vm);
            }

            if (emailConfiguration.Id == 0)
            {
                _emailConfigurationRepository.AddEmailConfiguration(emailConfiguration);
            }
            else
            {
                _emailConfigurationRepository.UpdateEmailConfiguration(emailConfiguration);

            };
            return RedirectToAction("Configurations");
        }



        public ActionResult SelectConfiguration()
        {
            var userId = User.Identity.GetUserId();

            var configurations = _emailConfigurationRepository.GetEmailConfigurations(userId);

            return View(configurations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectConfiguration(int emailConfigurationId)
        {
            if (emailConfigurationId == 0)
                return RedirectToAction("SelectConfiguration");

            Session["EmailConfigurationId"] = emailConfigurationId;
            return RedirectToAction("Folders", "Mailbox");
        }

        private EmailConfiguration GetNewConfiguration(string userId)
        {
            return new EmailConfiguration
            {
                UserId = userId,
                EmailAddress = new EmailAddress
                {
                    UserId = userId,
                    IsDefined = false
                }

            };
        }

        private EditEmailConfigurationViewModel PrepareEditEmailConfigurationViewModel(EmailConfiguration configuration)
        {

            var vm = new EditEmailConfigurationViewModel
            {
                EmailConfiguration = configuration,
                Heading = configuration.Id != 0 ? "Konfiguracja konta użytkownika" : "Dodawanie konta użytkownika"
            };

            return vm;
        }

        public ActionResult Configurations()
        {

            var userId = User.Identity.GetUserId();

            List<EmailConfiguration> configruations = _emailConfigurationRepository.GetEmailConfigurations(userId);

            return View(configruations);
        }

        [HttpPost]
        public ActionResult DeleteConfiguration(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _emailConfigurationRepository.DeleteEmailConfiguration(id, userId);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }


            return Json(new { Success = true });
        }



    }
}