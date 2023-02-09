using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailMessageAttachmentRepository
    {
        public void UpdateEmailMessageAttachments(int emailMessageId, string userId, ICollection<EmailAttachment> emailAttachments)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var messageAttachmentsFromDb = context.EmailAttachments.Where(x => x.UserId == userId
                                                                              && x.EmailMessageId == emailMessageId)
                                                                      .ToList();

                var attachmentsToDelete = messageAttachmentsFromDb.GroupJoin(emailAttachments,
                                                                             x => x.Id,
                                                                             y => y.Id,
                                                                             (x, y) => new { OldAttachment = x, NewAttachments = y })
                                                                  .SelectMany(x => x.NewAttachments.DefaultIfEmpty(),
                                                                              (y, z) => new
                                                                              {
                                                                                  OldAttachment = y.OldAttachment,
                                                                                  NewAttachment = z
                                                                              })
                                                                  .Where(x => x.NewAttachment == null)
                                                                  .Select(x => x.OldAttachment)
                                                                  .ToList();
                context.EmailAttachments.RemoveRange(attachmentsToDelete);

                var attachmentsToAdd = emailAttachments.GroupJoin(messageAttachmentsFromDb,
                                                                x => x.Id,
                                                                y => y.Id,
                                                                (x, y) => new
                                                                {
                                                                    NewAttachment = x,
                                                                    OldAttachments = y
                                                                })
                                                     .SelectMany(x => x.OldAttachments.DefaultIfEmpty(),
                                                                (y, z) => new
                                                                {
                                                                    NewAttachment = y.NewAttachment,
                                                                    OldAttachment = z
                                                                })
                                                     .Where(x => x.OldAttachment == null)
                                                     .Select(x => x.NewAttachment)
                                                     .ToList();
                        

                context.EmailAttachments.AddRange(attachmentsToAdd);

                context.SaveChanges();
            }
        }

        public ICollection<EmailAttachment> GetEmailMessageAttachments(ICollection<EmailAttachment> emailAttachments, int messageId, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var emailAttachmentsFromDb = context.EmailAttachments
                                                    .Where(x => x.EmailMessageId == messageId 
                                                             && x.UserId == userId)
                                                    .ToList();


                var updatedEmailAttachments = emailAttachmentsFromDb.Join(emailAttachments,
                                                                          x => x.Id,
                                                                          y => y.Id,
                                                                          (x, y) => new EmailAttachment
                                                                          {

                                                                              Id = x.Id,
                                                                              UserId = userId,
                                                                              FileName = x.FileName,
                                                                              EmailMessageId = x.EmailMessageId,
                                                                              FileStream = x.FileStream

                                                                          })
                                                                    .ToList();

                return updatedEmailAttachments;
            }
        }
    }
}