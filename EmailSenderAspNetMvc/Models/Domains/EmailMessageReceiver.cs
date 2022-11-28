using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailMessageReceiver
    {
        public int Id { get; set; }
        public int EmailMessageId { get; set; }
        public int EmailAddressId { get; set; }
        public EmailMessageReceiverType EmailMessageReceiverType { get; set; }

        public EmailMessage EmailMessage { get; set; }
        public EmailAddress EmailAddress { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}