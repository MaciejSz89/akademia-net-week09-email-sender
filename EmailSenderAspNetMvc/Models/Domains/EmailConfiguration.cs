using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailConfiguration
    {
        public EmailConfiguration()
        {
            EmailMessages = new Collection<EmailMessage>();
            EmailFolders = new Collection<EmailFolder>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [Display(Name = "Serwer SMTP")]
        public string SmtpHost { get; set; }
        

        [Required(ErrorMessage = "Pole jest wymagane")]
        [Display(Name = "Port SMTP")]
        public int SmtpPort { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [Display(Name = "Serwer IMAP")]
        public string ImapHost { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [Display(Name = "Port IMAP")]
        public int ImapPort { get; set; }


        [Required(ErrorMessage = "Pole jest wymagane")]
        [Display(Name = "Nazwa konta")]
        public string Name { get; set; }

        public int EmailAddressId { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        public byte[] PasswordHash { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public EmailAddress EmailAddress { get; set; }
        public ICollection<EmailFolder> EmailFolders { get; set; }

        public ICollection<EmailMessage> EmailMessages { get; set; }
    }
}