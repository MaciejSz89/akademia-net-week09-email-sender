using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailAttachmentRepository
    {
        public EmailAttachment GetAttachment(int attachmentId, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailAttachments
                              .Single(x => x.UserId == userId 
                                        && x.Id == attachmentId);
            }
        }

        

        public ICollection<EmailAttachment> GetAttachments(ICollection<EmailAttachment> emailAttachments, int messageId, string userId)
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