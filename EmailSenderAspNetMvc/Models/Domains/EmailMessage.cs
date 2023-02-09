using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            EmailMessageReceivers = new Collection<EmailMessageReceiver>();
            EmailAttachments = new Collection<EmailAttachment>();
            
        }
        public int Id { get; set; }

        

        [Display(Name = "Temat")]
        public string Subject { get; set; }

        [Display(Name = "Wiadomość")]
        [Required(ErrorMessage = "Nie można wysłać pustej wiadomości")]
        public string Content { get; set; }
        public bool IsBodyHtml { get; set; }        

        [Display(Name = "Data zapisu")]
        public DateTime? SaveDate { get; set; }

        [Display(Name = "Nadawca")]
        [Required(ErrorMessage = "Pole Nadawca jest wymagane")]
        public int EmailConfigurationId { get; set; }

        public EmailConfiguration EmailConfiguration { get; set; }

        [Display(Name = "Odbiorcy")]
        public ICollection<EmailMessageReceiver> EmailMessageReceivers { get; set; }
        public ICollection<EmailAttachment> EmailAttachments { get; set; }
       

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}