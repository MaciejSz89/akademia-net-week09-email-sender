using EmailSenderAspNetMvc.Models.Domains;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailMessageRepository
    {

        public void DeleteMessagesWithNoFolder(string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {


                var messagesToDeleteIds = context.EmailMessages.Where(x => x.UserId == userId
                                                                            && !context.EmailMessageFolderPairs
                                                                                       .Any(y => y.UserId == userId
                                                                                              && y.EmailMessageId == x.Id))
                                                                     .Select(x => x.Id).ToList();
                

                messagesToDeleteIds.ForEach(x =>
                {
                    var receiversToDelete = context.EmailMessageReceivers.Where(y => y.EmailMessageId == x);
                    var attachmentsToDelete = context.EmailAttachments.Where(y => y.EmailMessageId == x);

                    context.EmailMessageReceivers.RemoveRange(receiversToDelete);
                    context.EmailAttachments.RemoveRange(attachmentsToDelete);

                });

                var messagesToDelete = context.EmailMessages.Where(x => x.UserId == userId
                                                                            && !context.EmailMessageFolderPairs
                                                                                       .Any(y => y.UserId == userId
                                                                                              && y.EmailMessageId == x.Id));

                context.EmailMessages
                       .RemoveRange(messagesToDelete);

                context.SaveChanges();
            }
        }

        public void AddMessages(IList<EmailMessage> emailMessagesToAdd)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                foreach (var emailMessage in emailMessagesToAdd)
                {
                    var messageExists = context.EmailMessages.Any(x => x.ImapMessageId == emailMessage.ImapMessageId);

                    if (!messageExists)
                    {
                        context.EmailMessages.Add(emailMessage);
                        continue;
                    }

                    if (context.EmailMessages
                               .Single(x => x.ImapMessageId == emailMessage.ImapMessageId).Date != emailMessage.Date)
                        UpdateMessage(emailMessage, context);
                }
                context.SaveChanges();
            }
        }

        private void UpdateMessage(EmailMessage emailMessage,
                                         ApplicationDbContext context)
        {


            var messageToUpdate = context.EmailMessages.Single(x => x.ImapMessageId == emailMessage.ImapMessageId);

            var receiversToDelete = context.EmailMessageReceivers
                                           .Where(x => x.EmailMessageId == messageToUpdate.Id)
                                           .ToList();

            context.EmailMessageReceivers.RemoveRange(receiversToDelete);

            messageToUpdate.From = emailMessage.From;
            messageToUpdate.EmailMessageReceivers = emailMessage.EmailMessageReceivers;
            messageToUpdate.Date = emailMessage.Date;
            messageToUpdate.Subject = emailMessage.Subject;
            messageToUpdate.Flags = emailMessage.Flags;

        }

        public int GetMessageId(string messageId,
                                int emailConfigurationId,
                                string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailMessages
                              .Single(x => x.UserId == userId
                                        && x.EmailConfigurationId == emailConfigurationId
                                        && x.ImapMessageId == messageId)
                              .Id;
            }
        }

        public EmailMessage GetMessage(int messageId, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailMessages
                              .Include(x => x.From)
                              .Include(x => x.EmailMessageReceivers.Select(y => y.EmailAddress))
                              .Include(x => x.EmailAttachments)
                              .Single(x => x.UserId == userId
                                        && x.Id == messageId);
            }
        }
    }
}