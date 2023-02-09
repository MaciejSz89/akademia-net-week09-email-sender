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
    public class AddressController : Controller
    {
        EmailAddressRepository _emailAddressRepository = new EmailAddressRepository();

        public ActionResult EmailAddress(int id = 0)
        {
            var userId = User.Identity.GetUserId();

            var emailAddress = id == 0 ? GetNewDefinedEmailAddress(userId) :
                                          _emailAddressRepository.GetEmailAddress(id, userId);

            var vm = PrepareEditEmailAdressViewModel(emailAddress);

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailAddress(EmailAddress emailAddress)
        {
            var userId = User.Identity.GetUserId();
            emailAddress.UserId = userId;
            emailAddress.IsDefined = true;

            if (!ModelState.IsValid)
            {
                var vm = PrepareEditEmailAdressViewModel(emailAddress);
                return View("Configuration", vm);
            }

            if (emailAddress.Id == 0)
            {
                _emailAddressRepository.AddEmailAddress(emailAddress);
            }
            else
            {
                _emailAddressRepository.UpdateDefinedEmailAddress(emailAddress);

            };
            return RedirectToAction("EmailAddresses");
        }
        private EmailAddress GetNewDefinedEmailAddress(string userId)
        {
            return new EmailAddress
            {
                UserId = userId,
                IsDefined = true
            };
        }

        private EditEmailAddressViewModel PrepareEditEmailAdressViewModel(EmailAddress emailAddress)
        {

            var vm = new EditEmailAddressViewModel
            {
                EmailAddress = emailAddress,
                Heading = emailAddress.Id != 0 ? "Edycja adresu" : "Dodawanie adresu"
            };
            return vm;
        }



        public ActionResult EmailAddresses()
        {

            var userId = User.Identity.GetUserId();

            List<EmailAddress> emailAddresses = _emailAddressRepository.GetDefinedEmailAddresses(userId);

            return View(emailAddresses);
        }



        [HttpPost]
        public ActionResult DeleteEmailAddress(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _emailAddressRepository.DeleteDefinedEmailAddress(id, userId);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }


            return Json(new { Success = true });
        }
    }
}