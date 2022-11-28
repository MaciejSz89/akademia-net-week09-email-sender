using EmailSenderAspNetMvc.Models.Domains;
using EmailSenderAspNetMvc.Models.Repositories;
using EmailSenderAspNetMvc.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace EmailSenderAspNetMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        EmailConfigurationRepository _emailConfigurationRepository = new EmailConfigurationRepository();
        EmailAddressRepository _emailAddressRepository = new EmailAddressRepository();
        EmailMessageRepository _emailMessageRepository = new EmailMessageRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Configuration(int id = 0)
        {
            var userId = User.Identity.GetUserId();

            var configuration = id == 0 ? GetNewConfiguration(userId) :
                                          _emailConfigurationRepository.GetEmailConfiguration(id, userId);

            var vm = PrepareEditEmailConfigurationViewModel(configuration);

            return View(vm);
        }

        private EmailConfiguration GetNewConfiguration(string userId)
        {
            return new EmailConfiguration
            {
                UserId = userId,
                EmailAddress = new EmailAddress
                {
                    UserId = userId
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

        public ActionResult CreateMessage(int id = 0)
        {
            var userId = User.Identity.GetUserId();

            var message = id == 0 ? GetNewMessage(userId) : null;

            var configurations = _emailConfigurationRepository
                                    .GetEmailConfigurations(userId);

            var receivers = new List<EmailMessageReceiver>();

            var vm = PrepareSendEmailMessageViewModel(message, configurations, receivers);

            return View(vm);
        }



        private EditEmailMessageViewModel PrepareSendEmailMessageViewModel(EmailMessage message, 
                                                                           List<EmailConfiguration> configurations,
                                                                           List<EmailMessageReceiver> receivers)
        {
            var vm = new EditEmailMessageViewModel
            {
                EmailMessage = message,
                EmailConfigurations = configurations,
                EmailMessageReceivers = receivers,
                Heading = message.Id != 0 ? "Edycja wiadomości" : "Tworzenie nowej wiadomości"
            };

            return vm;
        }

        private EmailMessage GetNewMessage(string userId)
        {
            return new EmailMessage
            {
                UserId = userId,                
            };
        }

        public ActionResult Configurations()
        {

            var userId = User.Identity.GetUserId();

            List<EmailConfiguration> configruations = _emailConfigurationRepository.GetEmailConfigurations(userId);

            return View(configruations);
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

   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMessage(EmailMessage emailMessage, List<EmailMessageReceiver> emailReceivers, bool send, bool save)
        {          
                    
            var userId = User.Identity.GetUserId();


            if (send)
            {
                var emailConfiguration = _emailConfigurationRepository.GetEmailConfiguration(emailMessage.EmailConfigurationId,
                                                                                         userId);
                var smtpClient = new SmtpClient(emailConfiguration.Host,
                                                emailConfiguration.Port);

                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailConfiguration.EmailAddress.Address,
                                                               _emailConfigurationRepository.ReadEmailConfigurationPassword(emailConfiguration));

                var message = new MailMessage();
                message.Sender = new MailAddress(emailConfiguration.EmailAddress.Address,
                                               emailConfiguration.EmailAddress.DisplayName);
                message.From = new MailAddress(emailConfiguration.EmailAddress.Address,
                                               emailConfiguration.EmailAddress.DisplayName);

                foreach (var receiver in emailReceivers)
                {
                    message.To.Add(new MailAddress(receiver.EmailAddress.Address,
                                                    receiver.EmailAddress.DisplayName));
                }

                message.Subject = emailMessage.Subject;
                message.Body = emailMessage.Content;

                smtpClient.Send(message);

            }
            return RedirectToAction("About");
        }

        [HttpPost]
        public ActionResult MessageReceivers(List<EmailMessageReceiver> emailMessageReceivers)
        {
            if (emailMessageReceivers == null)
                emailMessageReceivers = new List<EmailMessageReceiver>();

            emailMessageReceivers.Add(new EmailMessageReceiver());
            return PartialView("_MessageReceivers", emailMessageReceivers);
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

        [HttpPost]
        public ActionResult DeleteEmailMessageReceiver(int messageId, int receiverId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _emailMessageRepository.DeleteEmailMessageReceiver(messageId, receiverId, userId);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }


            return Json(new { Success = true });
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Maciej Szafrański - Junior .NET Programmer.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Kontakt mailowy";

            return View();
        }
    }
}