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
    public class EmailAddress
    {

        public EmailAddress()
        {
            EmailConfigurations = new Collection<EmailConfiguration>();
            EmailMessageReceivers = new Collection<EmailMessageReceiver>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [Display(Name = "Adres email")]
        [EmailAddress]
        public string Address { get; set; }

        [Display(Name = "Wyświetlana nazwa")]
        public string DisplayName { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public bool IsDefined { get; set; }

        [NotMapped]
        public string AddressInfo
        {
            get
            {
                return DisplayName + " <" + Address + ">";
            }
        }

        public ApplicationUser User { get; set; }

        public ICollection<EmailConfiguration> EmailConfigurations { get; set; }

        public ICollection<EmailMessageReceiver> EmailMessageReceivers { get; set; }
    }
}