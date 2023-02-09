using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class DisplayEmailFolderMessagesViewModel
    {
        public EmailFolder EmailFolder { get; set; }
        public List<EmailFolderMessage> EmailMessageFolderMessages { get; set; }
    }
}