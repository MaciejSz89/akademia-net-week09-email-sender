using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class EditEmailMessageViewModel
    {
        public EmailMessage EmailMessage { get; set; }
        public List<EmailConfiguration> EmailConfigurations { get; set; }
        public List<EmailMessageReceiver> EmailMessageReceivers { get; set; }
        public string Heading { get; set; }

        public bool Send { get; set; }
        public bool Save { get; set; }
    }
}