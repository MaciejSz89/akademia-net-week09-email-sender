﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using EmailSenderAspNetMvc.Models.Domains;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EmailSenderAspNetMvc.Models.Domains
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public ApplicationUser()
        {
            EmailAddresses = new Collection<EmailAddress>();
            EmailAttachments = new Collection<EmailAttachment>();
            EmailConfigurations = new Collection<EmailConfiguration>();
            EmailMessages = new Collection<EmailMessage>();
            EmailMessageReceivers = new Collection<EmailMessageReceiver>();
            EmailFolderAttachments = new Collection<EmailFolderAttachment>();
        }

        public string Name { get; set; }

        public ICollection<EmailAddress> EmailAddresses { get; set; }
        public ICollection<EmailAttachment> EmailAttachments { get; set; }
        public ICollection<EmailConfiguration> EmailConfigurations { get; set; }
        public ICollection<EmailMessage> EmailMessages { get; set; }
        public ICollection<EmailMessageReceiver> EmailMessageReceivers { get; set; }
        public ICollection<EmailFolder> EmailFolders { get; set; }
        public ICollection<EmailMessageFolderPair> EmailMessageFolderPairs { get; set; }
        public ICollection<EmailFolderAttachment> EmailFolderAttachments { get; set; }



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

   
    }
}