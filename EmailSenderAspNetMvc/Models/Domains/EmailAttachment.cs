using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailAttachment
    {
        public int Id { get; set; }
        public byte[] Attachment { get; set; }

        public int EmailMessageId { get; set; }


        public EmailMessage EmailMessage { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }


        public ApplicationUser User { get; set; }
    }
}