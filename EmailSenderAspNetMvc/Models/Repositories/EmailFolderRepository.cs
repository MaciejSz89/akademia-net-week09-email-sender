using EmailSenderAspNetMvc.Models.Domains;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailFolderRepository
    {
        EmailConfigurationRepository _emailConfigurationRepository = new EmailConfigurationRepository();

        public List<EmailFolder> GetFolders(EmailConfiguration emailConfiguration)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var folders = context.EmailFolders.Where(x => x.UserId == emailConfiguration.UserId
                                                            && x.EmailConfigurationId == emailConfiguration.Id
                                                            && !x.Attributes.HasFlag(FolderAttributes.NonExistent)
                                                            && (x.ParentFolder == null
                                                               || x.ParentFolder.Attributes.HasFlag(FolderAttributes.NonExistent)))
                                                  .Include(x => x.EmailFolders)
                                                  .ToList()
                                                  .OrderBy(x => x, new EmailFolderComparer())
                                                  .ToList();

                return folders;
            }
        }

        public void UpdateFolders(int emailConfigurationId, string userId, List<EmailFolder> emailFolders)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var foldersFromDb = context.EmailFolders
                                           .Where(x => x.UserId == userId
                                                    && x.EmailConfigurationId == emailConfigurationId)
                                           .ToList();

                var foldersToDelete = foldersFromDb.GroupJoin(emailFolders,
                                                                     x => x.FullName,
                                                                     y => y.FullName,
                                                                     (x, y) => new { OldFolder = x, NewFolders = y })
                                                   .SelectMany(x => x.NewFolders.DefaultIfEmpty(),
                                                              (y, z) => new { OldFolder = y.OldFolder, NewFolder = z })
                                                   .Where(x => x.NewFolder == null)
                                                   .Select(x => x.OldFolder)
                                                   .ToList();


                foldersToDelete.ForEach(x =>
                                       {
                                           var emailFolderMessagePairsToDelete = context.EmailMessageFolderPairs
                                                                                        .Where(y => y.UserId == userId
                                                                                                 && y.EmailFolderId == x.Id)
                                                                                        .ToList();
                                           emailFolderMessagePairsToDelete.ForEach(y =>
                                                                                    {
                                                                                        context.EmailMessageFolderPairs.Remove(y);
                                                                                    });
                                           context.EmailFolders.Remove(x);
                                       });

                var foldersToAdd = emailFolders.GroupJoin(foldersFromDb,
                                                                     x => x.FullName,
                                                                     y => y.FullName,
                                                                     (x, y) => new { NewFolder = x, OldFolders = y })
                                                          .SelectMany(x => x.OldFolders.DefaultIfEmpty(),
                                                                     (y, z) => new { NewFolder = y.NewFolder, OldFolder = z })
                                                          .Where(x => x.OldFolder == null)
                                                          .Select(x => x.NewFolder)
                                                          .ToList();

                foldersToAdd.ForEach(x =>
                {
                    x.EmailConfigurationId = emailConfigurationId;
                    context.EmailFolders.Add(x);
                });

                var foldersToUpdate = emailFolders.GroupJoin(foldersFromDb,
                                                                     x => x.FullName,
                                                                     y => y.FullName,
                                                                     (x, y) => new { NewFolder = x, OldFolders = y })
                                                          .SelectMany(x => x.OldFolders.DefaultIfEmpty(),
                                                                     (y, z) => new
                                                                     {
                                                                         NewFolder = y.NewFolder,
                                                                         OldFolder = z
                                                                     })
                                                          .Where(x => x.OldFolder != null)
                                                          .Select(x => x)
                                                          .ToList();

                foldersToUpdate.ForEach(x =>
                {
                    x.OldFolder.Attributes = x.NewFolder.Attributes;
                    x.OldFolder.Name = x.NewFolder.Name;
                    x.OldFolder.ParentFolderName = x.NewFolder.ParentFolderName;
                    x.OldFolder.EmailConfigurationId = x.NewFolder.EmailConfigurationId;
                });



                context.SaveChanges();

                foldersFromDb = context.EmailFolders
                                             .Where(x => x.UserId == userId
                                                      && x.EmailConfigurationId == emailConfigurationId)
                                             .ToList();


                foreach (var folder in foldersFromDb)
                {
                    folder.ParentFolder = foldersFromDb.SingleOrDefault(x => x.FullName == folder.ParentFolderName);
                }

                context.SaveChanges();

            }
        }



        public List<EmailFolderMessage> GetFolderMessages(int folderId, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var emailFolderMessagePairs = context.EmailMessageFolderPairs
                                                     .Where(x => x.UserId == userId
                                                              && x.EmailFolderId == folderId)
                                                     .AsQueryable();

                var emailFolderMessages = context.EmailFolderMessages
                                                 .Where(x => x.UserId == userId)
                                                 .Join(emailFolderMessagePairs,
                                                       x => x.Id,
                                                       y => y.EmailFolderMessageId,
                                                       (x, y) => x)
                                                 .Include(x => x.From)
                                                 .Include(x => x.EmailFolderMessageReceivers.Select(r => r.EmailAddress))
                                                 .OrderByDescending(x => x.Date)
                                                 .ToList();


                return emailFolderMessages;
            }
        }

        public EmailFolder GetFolder(int folderId, string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailFolders.Single(x => x.UserId == userId
                                                     && x.Id == folderId);
            }
        }

        //public List<long> GetFolderMessageUids(string folderFullName, string userId)
        //{
        //    using (ApplicationDbContext context = new ApplicationDbContext())
        //    {
        //        var folder = context.EmailFolders.Single(x => x.UserId == userId
        //                                                 && x.FullName == folderFullName);

        //        return context.EmailMessageFolderPairs.Where(x => x.UserId == userId
        //                                                       && x.EmailFolderId == folder.Id)
        //                                              .Select(x => x.Uid)
        //                                              .ToList();
        //    }
        //}

        public int GetFolderId(string folderFullName,
                               int emailConfigurationId,
                               string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.EmailFolders.Single(x => x.UserId == userId
                                                     && x.EmailConfigurationId == emailConfigurationId
                                                     && x.FullName == folderFullName).Id;
            }
        }

    
    }
}