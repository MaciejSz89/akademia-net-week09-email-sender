using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailFolderAttachmentRepository
    {
        public EmailFolderAttachment GetAttachment(int attachmentId, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailFolderAttachments
                              .Single(x => x.UserId == userId 
                                        && x.Id == attachmentId);
            }
        }
    }
}