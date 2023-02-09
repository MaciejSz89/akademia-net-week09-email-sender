using EmailSenderAspNetMvc.Models.Domains;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailAddressRepository
    {
        public void AddEmailAddress(EmailAddress emailAddress)
        {
            using (var context = new ApplicationDbContext())
            {

                context.EmailAddresses
                       .Add(emailAddress);
                context.SaveChanges();

            }
        }


        public void DeleteNotDefinedEmailAddress(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var addressToDelete = context.EmailAddresses
                                                   .Include(x => x.EmailConfigurations)
                                                   .Include(x => x.EmailMessageReceivers)
                                                   .Single(x => x.UserId == userId
                                                             && !x.IsDefined
                                                             && x.Id == id);
                if ((addressToDelete.EmailConfigurations == null || addressToDelete.EmailConfigurations.Count == 0)
                 && (addressToDelete.EmailMessageReceivers == null || addressToDelete.EmailMessageReceivers.Count == 0))
                    context.EmailAddresses
                           .Remove(addressToDelete);

                context.SaveChanges();
            }
        }



        public void DeleteDefinedEmailAddress(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var addressToDelete = context.EmailAddresses
                                                   .Include(x => x.EmailConfigurations)
                                                   .Include(x => x.EmailMessageReceivers)
                                                   .Single(x => x.UserId == userId
                                                             && x.IsDefined
                                                             && x.Id == id);

                if (addressToDelete.EmailConfigurations != null && addressToDelete.EmailConfigurations.Count != 0)
                    foreach (var configuration in addressToDelete.EmailConfigurations)
                    {
                        configuration.EmailAddressId = 0;
                        configuration.EmailAddress = new EmailAddress
                        {
                            UserId = userId,
                            DisplayName = addressToDelete.DisplayName,
                            Address = addressToDelete.Address
                        };
                    }
                if (addressToDelete.EmailMessageReceivers != null && addressToDelete.EmailMessageReceivers.Count != 0)
                    foreach (var receiver in addressToDelete.EmailMessageReceivers)
                    {
                        receiver.EmailAddressId = 0;
                        receiver.EmailAddress = new EmailAddress
                        {
                            UserId = userId,
                            DisplayName = addressToDelete.DisplayName,
                            Address = addressToDelete.Address
                        };
                    }
                context.EmailAddresses
                       .Remove(addressToDelete);

                context.SaveChanges();
            }
        }

        public void DeleteAllNotReferencedEmailAddresses(string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {


                var addressesToDelete = context.EmailAddresses.Where(x => x.UserId == userId
                                                                       && x.EmailConfigurations.Count == 0
                                                                       && x.EmailMessageReceivers.Count == 0
                                                                       && x.EmailFolderMessageReceivers.Count == 0
                                                                       && !context.EmailFolderMessages.Any(y => y.UserId == userId
                                                                                                             && y.FromId == x.Id)
                                                                       && !x.IsDefined);
                context.EmailAddresses
                       .RemoveRange(addressesToDelete);
            
                context.SaveChanges();
            }
        }

        public void UpdateNotDefinedEmailAddress(EmailAddress address)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var addressesToUpdate = context.EmailAddresses.Single(x => x.UserId == address.UserId
                                                                        && x.Id == address.Id
                                                                        && !x.IsDefined);

                addressesToUpdate.Address = address.Address;
                addressesToUpdate.DisplayName = address.DisplayName;

                context.SaveChanges();
            }
        }

        public void UpdateDefinedEmailAddress(EmailAddress address)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var addressesToUpdate = context.EmailAddresses.Single(x => x.UserId == address.UserId
                                                                        && x.Id == address.Id
                                                                        && x.IsDefined);

                addressesToUpdate.Address = address.Address;
                addressesToUpdate.DisplayName = address.DisplayName;

                context.SaveChanges();
            }
        }

        public EmailAddress GetEmailAddress(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailAddresses.Single(x => x.UserId == userId
                                                                       && x.Id == id);
            }
        }

        public List<EmailAddress> GetDefinedEmailAddresses(string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailAddresses.Where(x => x.UserId == userId && x.IsDefined)
                                             .OrderBy(x => x.DisplayName)
                                             .ToList();
            }
        }
    }
}