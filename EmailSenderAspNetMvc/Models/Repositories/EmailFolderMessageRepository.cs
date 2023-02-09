using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailFolderMessageRepository
    {

        public void DeleteFolderMessagesWithNoFolder(string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {


                var messagesToDelete = context.EmailFolderMessages.Where(x => x.UserId == userId
                                                                            && !context.EmailMessageFolderPairs
                                                                                       .Any(y => y.UserId == userId
                                                                                              && y.EmailFolderMessageId == x.Id));
                context.EmailFolderMessages
                       .RemoveRange(messagesToDelete);

                context.SaveChanges();
            }
        }

        public void AddFolderMessages(IList<EmailFolderMessage> emailFolderMessagesToAdd)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                foreach (var emailFolderMessage in emailFolderMessagesToAdd)
                {
                    var messageExists = context.EmailFolderMessages.Any(x => x.ImapMessageId == emailFolderMessage.ImapMessageId);

                    if (!messageExists)
                    {
                        context.EmailFolderMessages.Add(emailFolderMessage);
                        continue;
                    }

                    if (context.EmailFolderMessages
                               .Single(x => x.ImapMessageId == emailFolderMessage.ImapMessageId).Date != emailFolderMessage.Date)
                        UpdateFolderMessage(emailFolderMessage, context);
                }
                context.SaveChanges();
            }
        }

        private void UpdateFolderMessage(EmailFolderMessage emailFolderMessage,
                                         ApplicationDbContext context)
        {


            var messageToUpdate = context.EmailFolderMessages.Single(x => x.ImapMessageId == emailFolderMessage.ImapMessageId);

            var receiversToDelete = context.EmailFolderMessageReceivers
                                           .Where(x => x.EmailFolderMessageId == messageToUpdate.Id)
                                           .ToList();

            context.EmailFolderMessageReceivers.RemoveRange(receiversToDelete);

            messageToUpdate.From = emailFolderMessage.From;
            messageToUpdate.EmailFolderMessageReceivers = emailFolderMessage.EmailFolderMessageReceivers;
            messageToUpdate.Date = emailFolderMessage.Date;
            messageToUpdate.Subject = emailFolderMessage.Subject;
            messageToUpdate.Flags = emailFolderMessage.Flags;

        }

        public int GetMessageId(string messageId,
                                int emailConfigurationId,
                                string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailFolderMessages
                              .Single(x => x.UserId == userId
                                        && x.EmailConfigurationId == emailConfigurationId
                                        && x.ImapMessageId == messageId)
                              .Id;
            }
        }

        public EmailFolderMessage GetMessage(int messageId, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailFolderMessages
                              .Include(x => x.From)
                              .Include(x => x.EmailFolderMessageReceivers.Select(y => y.EmailAddress))
                              .Include(x => x.EmailFolderAttachments)
                              .Single(x => x.UserId == userId
                                        && x.Id == messageId);
            }
        }
    }
}