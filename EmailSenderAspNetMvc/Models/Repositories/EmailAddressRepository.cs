using EmailSenderAspNetMvc.Models.Domains;
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

       
        public void DeleteEmailAddress(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var addressToDelete = context.EmailAddresses
                                                   .Include(x => x.EmailConfigurations)
                                                   .Include(x => x.EmailMessageReceivers)
                                                   .Single(x => x.UserId == userId
                                                             && x.Id == id);
                if ((addressToDelete.EmailConfigurations==null || addressToDelete.EmailConfigurations.Count==0)
                 && (addressToDelete.EmailMessageReceivers == null || addressToDelete.EmailMessageReceivers.Count == 0))
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
                                                                       && x.EmailMessageReceivers.Count == 0);

                foreach (var address in addressesToDelete)
                {
                    context.EmailAddresses
                           .Remove(address);
                }

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

        public EmailAddress GetDefinedEmailAddress(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailAddresses.Single(x => x.UserId == userId
                                                                       && x.Id == id
                                                                       && x.IsDefined);
            }
        }

   
    }
}