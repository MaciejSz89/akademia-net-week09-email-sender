using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class EditEmailMessageReceiversViewModel
    {
        [Display(Name = "Do")]
        public List<EmailMessageReceiver> EmailMessageReceiversTo { get; set; }

        [Display(Name = "DW")]
        public List<EmailMessageReceiver> EmailMessageReceiversCc { get; set; }

        [Display(Name = "UDW")]
        public List<EmailMessageReceiver> EmailMessageReceiversBcc { get; set; }

    }
}