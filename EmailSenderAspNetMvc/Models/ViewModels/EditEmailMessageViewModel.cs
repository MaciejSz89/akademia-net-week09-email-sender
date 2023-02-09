using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class EditEmailMessageViewModel
    {
        public EmailMessage EmailMessage { get; set; }
        public List<EmailConfiguration> EmailConfigurations { get; set; }

        [Display(Name = "Do")]
        public List<EmailMessageReceiver> EmailMessageReceiversTo { get; set; }

        [Display(Name = "DW")]
        public List<EmailMessageReceiver> EmailMessageReceiversCc { get; set; }

        [Display(Name = "UDW")]
        public List<EmailMessageReceiver> EmailMessageReceiversBcc { get; set; }

        [Display(Name = "Załączniki")]
        public List<HttpPostedFileBase> Attachments { get; set; }
        public List<EmailAddress> DefinedEmailAddresses { get; set; }

        public string Heading { get; set; }

        public bool Send { get; set; }
        public bool Save { get; set; }
    }
}