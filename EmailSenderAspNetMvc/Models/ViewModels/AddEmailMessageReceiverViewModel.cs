using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmailSenderAspNetMvc.Models.Domains;

namespace EmailSenderAspNetMvc.Models.ViewModels
{
    public class AddEmailMessageReceiverViewModel
    {
        public int ItemIndex { get; set; }
        public int MessageId { get; set; }
        public EmailMessageReceiverType ReceiverType { get; set; }
        public EmailAddress EmailAddress { get; set; }
    }
}