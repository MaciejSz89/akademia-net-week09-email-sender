using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailMessageRepository
    {
        EmailAddressRepository _emailAddressRepository = new EmailAddressRepository();
        EmailMessageAttachmentRepository _emailMessageAttachmentRepository = new EmailMessageAttachmentRepository();
        EmailMessageReceiverRepository _emailMessageReceiverRepository = new EmailMessageReceiverRepository();
        public void DeleteEmailMessageReceiver(int messageId, int receiverId, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var receiverToDelete = context.EmailMessageReceivers.Single(x => x.UserId == userId
                                                                              && x.EmailMessageId == messageId
                                                                              && x.Id == receiverId);


                context.EmailMessageReceivers.Remove(receiverToDelete);

                context.SaveChanges();
            }

            _emailAddressRepository.DeleteAllNotReferencedEmailAddresses(userId);

        }

        public void AddEmailMessage(EmailMessage emailMessage)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                context.EmailMessages.Add(emailMessage);
                context.SaveChanges();
            }
        }

        public void UpdateEmailMessage(EmailMessage emailMessage)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                _emailMessageReceiverRepository.UpdateEmailMessageReceivers(emailMessage.Id,
                                                                            emailMessage.UserId,
                                                                            emailMessage.EmailMessageReceivers);

                _emailMessageAttachmentRepository.UpdateEmailMessageAttachments(emailMessage.Id,
                                                                                emailMessage.UserId,
                                                                                emailMessage.EmailAttachments);
                var messageToUpdate = context.EmailMessages
                                                        .Include(x => x.EmailMessageReceivers)
                                                        .Include(x => x.EmailAttachments)
                                                        .Single(x => x.UserId == emailMessage.UserId
                                                                  && x.Id == emailMessage.Id);



                foreach (var attachment in emailMessage.EmailAttachments)
                {
                    if (attachment.Id != 0)
                    {
                        var attachmentToUpdate = messageToUpdate.EmailAttachments
                                                    .Single(x => x.Id == attachment.Id);
                        attachmentToUpdate.FileName = attachment.FileName;
                    }
                    else
                    {
                        messageToUpdate.EmailAttachments.Add(attachment);
                    }
                }

                messageToUpdate.EmailConfigurationId = emailMessage.EmailConfigurationId;
                messageToUpdate.Subject = emailMessage.Subject;
                messageToUpdate.Content = emailMessage.Content;
                messageToUpdate.SaveDate = DateTime.Now;

                context.SaveChanges();


                _emailAddressRepository.DeleteAllNotReferencedEmailAddresses(emailMessage.UserId);
            }


        }

        public List<EmailMessage> GetDraftEmailMessages(string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var messages = context.EmailMessages.Where(x => x.UserId == userId)
                                      .Include(x => x.EmailConfiguration)
                                      .Include(x => x.EmailConfiguration.EmailAddress)
                                      .Include(x => x.EmailMessageReceivers)
                                      .Include(x => x.EmailMessageReceivers.Select(y => y.EmailAddress))
                                      .Include(x => x.EmailAttachments)
                                      .ToList();

                messages.ForEach(x =>
                {
                    x.EmailMessageReceivers = x.EmailMessageReceivers
                                               .OrderBy(y => y.EmailMessageReceiverType)
                                               .ToList();
                });

                return messages;
            }
        }

        public void DeleteEmailMessage(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var messageToDelete = context.EmailMessages
                                             .Include(x => x.EmailMessageReceivers)
                                             .Single(x => x.UserId == userId
                                                       && x.Id == id);



                context.EmailMessages
                       .Remove(messageToDelete);

                context.SaveChanges();
            }

            _emailAddressRepository.DeleteAllNotReferencedEmailAddresses(userId);

        }

        public EmailMessage GetEmailMessage(int id, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var message = context.EmailMessages
                                           .Include(x => x.EmailMessageReceivers)
                                           .Include(x => x.EmailMessageReceivers.Select(y => y.EmailAddress))
                                           .Include(x => x.EmailAttachments)
                                           .Include(x => x.EmailConfiguration)
                                           .Include(x => x.EmailConfiguration.EmailAddress)
                                           .Single(x => x.UserId == userId
                                                     && x.Id == id);

                return message;

            }
        }
    }
}

