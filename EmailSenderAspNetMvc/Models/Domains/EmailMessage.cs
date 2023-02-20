using MailKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mail;
using System.Xml.Linq;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailMessage
    {

        public EmailMessage()
        {
            EmailMessageFolderPairs = new Collection<EmailMessageFolderPair>();
            EmailMessageReceivers = new Collection<EmailMessageReceiver>();
            EmailAttachments = new Collection<EmailAttachment>();
        }

        public int Id { get; set; }

        [Required]
        public string ImapMessageId { get; set; }

        [Display(Name = "Temat")]
        public string Subject { get; set; }

        [Display(Name = "Wiadomość")]
        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }

        public int FromId { get; set; }

        public int EmailConfigurationId { get; set; }

        public MessageFlags? Flags { get; set; }

        public ICollection<EmailMessageReceiver> EmailMessageReceivers { get; set; }

        public ICollection<EmailAttachment> EmailAttachments { get; set; }

        public ICollection<EmailMessageFolderPair> EmailMessageFolderPairs { get; set; }
        public DateTimeOffset Date { get; set; }

        public EmailConfiguration EmailConfiguration { get; set; }
        public EmailAddress From { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}