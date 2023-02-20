using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class DisplayEmailMessageViewModel
    {
        public EmailFolder EmailFolder { get; set; }

        public EmailMessage EmailMessage { get; set; }
    }
}