using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public void UpdateEmailAddress(EmailAddress emailAddress)
        {
            using (var context = new ApplicationDbContext())
            {
                var emailAddressToUpdate = context.EmailAddresses
                                                  .Where(x => x.UserId == emailAddress.UserId
                                                           && x.Id == emailAddress.Id)
                                                  .Single();

                emailAddressToUpdate.Address = emailAddress.Address;
                emailAddressToUpdate.DisplayName = emailAddress.DisplayName;
                context.SaveChanges();

            }
        }

        public void DeleteEmailAddress(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var addressToDelete = context.EmailAddresses
                                                   .Single(x => x.UserId == userId
                                                             && x.Id == id);
                context.EmailAddresses
                       .Remove(addressToDelete);

                context.SaveChanges();


            }
        }
    }
}