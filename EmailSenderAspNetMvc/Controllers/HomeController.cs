using EmailSenderAspNetMvc.Models.Domains;
using EmailSenderAspNetMvc.Models.Repositories;
using EmailSenderAspNetMvc.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmailSenderAspNetMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        EmailConfigurationRepository _emailConfigurationRepository = new EmailConfigurationRepository();
        EmailAddressRepository _emailAddressRepository = new EmailAddressRepository();
        EmailMessageRepository _emailMessageRepository = new EmailMessageRepository();
        EmailMessageAttachmentRepository _emailMessageAttachmentRepository = new EmailMessageAttachmentRepository();

        public ActionResult Index()
        {
            return View();
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