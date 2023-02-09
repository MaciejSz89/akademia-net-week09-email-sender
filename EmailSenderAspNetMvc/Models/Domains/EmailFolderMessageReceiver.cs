using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailFolderMessageReceiver
    {
        public int Id { get; set; }
        public int EmailFolderMessageId { get; set; }
        public int EmailAddressId { get; set; }
        public EmailMessageReceiverType EmailMessageReceiverType { get; set; }

        public EmailFolderMessage EmailFolderMessage { get; set; }
        public EmailAddress EmailAddress { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}