using EmailSenderAspNetMvc.Models.Domains;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailConfigurationRepository
    {
        EmailAddressRepository _emailAddressRepository = new EmailAddressRepository();
        public EmailConfiguration GetEmailConfiguration(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var configuration = context.EmailConfigurations
                                           .Include(x => x.EmailAddress)
                                           .Single(x => x.UserId == userId
                                                     && x.Id == id);

                return configuration;

            }
        }

        public List<EmailConfiguration> GetEmailConfigurations(string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var configurations = context.EmailConfigurations
                                            .Include(x => x.EmailAddress)
                                            .Where(x => x.UserId == userId)
                                            .ToList();

                return configurations;

            }
        }

        public void AddEmailConfiguration(EmailConfiguration configuration)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                CreatePasswordHash(configuration);
                ProtectPassword(configuration);
                context.EmailConfigurations.Add(configuration);
                context.SaveChanges();
            }
        }

        public void UpdateEmailConfiguration(EmailConfiguration configuration)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var configurationToUpdate = context.EmailConfigurations
                                                   .Single(x => x.UserId == configuration.UserId
                                                             && x.Id == configuration.Id);


                configurationToUpdate.EmailAddress = new EmailAddress
                {
                    UserId = configuration.UserId,
                    Address = configuration.EmailAddress.Address,
                    DisplayName = configuration.EmailAddress.DisplayName
                };

                var oldAddressId = configurationToUpdate.EmailAddressId;
                configurationToUpdate.EmailAddressId = 0;

                configurationToUpdate.Password = configuration.Password;
                ProtectPassword(configurationToUpdate);

                configurationToUpdate.Host = configuration.Host;
                configurationToUpdate.Port = configuration.Port;
                context.SaveChanges();

                _emailAddressRepository.DeleteEmailAddress(oldAddressId, configuration.UserId);
            }
        }



        private void ProtectPassword(EmailConfiguration configuration)
        {
            DataProtectionScope scope = DataProtectionScope.CurrentUser;
            var unprotectedPasswordHash = ProtectedData.Unprotect(configuration.PasswordHash, null, scope);
            var cipher = new Cipher.StringCipher(unprotectedPasswordHash);
            configuration.Password = cipher.Encrypt(configuration.Password);
        }

        private void CreatePasswordHash(EmailConfiguration configuration)
        {
            DataProtectionScope scope = DataProtectionScope.CurrentUser;
            byte[] unprotectedPasswordHash = new byte[16];
            RandomNumberGenerator rand = RandomNumberGenerator.Create();
            rand.GetBytes(unprotectedPasswordHash);
            configuration.PasswordHash = ProtectedData.Protect(unprotectedPasswordHash, null, scope);
        }

        public string ReadEmailConfigurationPassword(EmailConfiguration configuration)
        {
            DataProtectionScope scope = DataProtectionScope.CurrentUser;
            var unprotectedPasswordHash = ProtectedData.Unprotect(configuration.PasswordHash, null, scope);
            var cipher = new Cipher.StringCipher(unprotectedPasswordHash);
            var encryptedPassword = configuration.Password;
            return cipher.Decrypt(encryptedPassword);
        }

        public void DeleteEmailConfiguration(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var configurationToDelete = context.EmailConfigurations
                                                   .Single(x => x.UserId == userId
                                                             && x.Id == id);


                var addressToDeleteId = context.EmailAddresses
                                             .Include(x => x.EmailConfigurations)
                                             .Single(x => x.UserId == userId
                                                       && x.Id == configurationToDelete.EmailAddressId)
                                             .Id;



                context.EmailConfigurations
                       .Remove(configurationToDelete);

                context.SaveChanges();

                _emailAddressRepository.DeleteEmailAddress(addressToDeleteId, userId);

            }
        }
    }
}