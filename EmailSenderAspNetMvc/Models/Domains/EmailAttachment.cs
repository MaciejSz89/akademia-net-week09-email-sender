using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public byte[] FileStream { get; set; }

        public int EmailMessageId { get; set; }

        public EmailMessage EmailMessage { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}