using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class DisplayEmailMessagesViewModel
    {
        public EmailFolder EmailFolder { get; set; }
        public List<EmailMessage> EmailMessages { get; set; }
    }
}