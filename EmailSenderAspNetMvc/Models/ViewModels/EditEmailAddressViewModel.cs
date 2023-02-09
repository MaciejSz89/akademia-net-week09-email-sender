using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class EditEmailAddressViewModel
    {
        public EmailAddress EmailAddress { get; set; }
        public string Heading { get; set; }
    }
}