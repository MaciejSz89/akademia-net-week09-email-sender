using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class EditEmailConfigurationViewModel
    {
        public EmailConfiguration EmailConfiguration { get; set; }
        public string Heading { get; set; }
    }
}