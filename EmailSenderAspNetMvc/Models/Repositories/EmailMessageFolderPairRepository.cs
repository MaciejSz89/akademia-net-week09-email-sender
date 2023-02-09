using EmailSenderAspNetMvc.Models.Domains;
using MailKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.UI;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailMessageFolderPairRepository
    {


        public void DeleteOldFolderMessagePairs(IDictionary<string, IList<long>> newFolderMessagePairsDictionary,
                                                int emailConfigurationId,
                                                string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                foreach (var keyValuePair in newFolderMessagePairsDictionary)
                {
                    var folderId = context.EmailFolders.Single(x => x.UserId == userId
                                                                 && x.EmailConfigurationId == emailConfigurationId
                                                                 && x.FullName == keyValuePair.Key)
                                                       .Id;
                    var uids = keyValuePair.Value;

                    var folderMessagePairsFromDb = context.EmailMessageFolderPairs
                                                          .Where(x => x.EmailFolderId == folderId)
                                                          .ToList();

                    var folderMessagePairsToDelete = folderMessagePairsFromDb.GroupJoin(uids,
                                                                                        x => x.ImapFolderMessageUid,
                                                                                        y => y,
                                                                                        (x, y) => new
                                                                                        {
                                                                                            OldImapMessageFolderPair = x,
                                                                                            NewImapFolderMessageUids = y
                                                                                        })
                                                                             .SelectMany(x => x.NewImapFolderMessageUids.DefaultIfEmpty(),
                                                                                     (y, z) => new
                                                                                     {
                                                                                         OldImapMessageFolderPair = y.OldImapMessageFolderPair,
                                                                                         NewImapFolderMessageUid = z
                                                                                     })
                                                                             .Where(x => x.NewImapFolderMessageUid == 0)
                                                                             .Select(x => x.OldImapMessageFolderPair);

                    context.EmailMessageFolderPairs.RemoveRange(folderMessagePairsToDelete);

                }

                context.SaveChanges();
            }
        }

        public IDictionary<string, IList<long>> GetNewFolderMessagePairsUids(IDictionary<string, IList<long>> newFolderMessagePairsDictionary,
                                                        int emailConfigurationId,
                                                        string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var newFolderMessagePairsUids = new Dictionary<string, IList<long>>();

                foreach (var keyValuePair in newFolderMessagePairsDictionary)
                {
                    var folderId = context.EmailFolders.Single(x => x.UserId == userId
                                                                 && x.EmailConfigurationId == emailConfigurationId
                                                                 && x.FullName == keyValuePair.Key)
                                                       .Id;
                    var uids = keyValuePair.Value;

                    var folderMessagePairsFromDb = context.EmailMessageFolderPairs
                                                          .Where(x => x.EmailFolderId == folderId)
                                                          .ToList();

                    var newUids = uids.GroupJoin(folderMessagePairsFromDb,
                                               x => x,
                                               y => y.ImapFolderMessageUid,
                                               (x, y) => new
                                               {
                                                   NewImapMessageUid = x,
                                                   OldImapMessageFolderPairs = y
                                               })
                                      .SelectMany(x => x.OldImapMessageFolderPairs.DefaultIfEmpty(),
                                                  (y, z) => new
                                                  {
                                                      NewImapMessageUid = y.NewImapMessageUid,
                                                      OldImapMessageFolderPair = z
                                                  })
                                      .Where(x => x.OldImapMessageFolderPair == default)
                                      .Select(x => x.NewImapMessageUid)
                                      .ToList();

                    newFolderMessagePairsUids.Add(keyValuePair.Key, newUids);
                }

                return newFolderMessagePairsUids;

            }
        }

        public IList<long> GetNewFolderMessageUids(string folderFullName,
                                                   IList<long> uids,
                                                   int emailConfigurationId,
                                                   string userId)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var folderId = context.EmailFolders.Single(x => x.UserId == userId
                                                             && x.EmailConfigurationId == emailConfigurationId
                                                             && x.FullName == folderFullName)
                                                   .Id;


                var folderMessagePairsFromDb = context.EmailMessageFolderPairs
                                                      .Where(x => x.EmailFolderId == folderId)
                                                      .ToList();

                var newUids = uids.GroupJoin(folderMessagePairsFromDb,
                                           x => x,
                                           y => y.ImapFolderMessageUid,
                                           (x, y) => new
                                           {
                                               NewImapMessageUid = x,
                                               OldImapMessageFolderPairs = y
                                           })
                                  .SelectMany(x => x.OldImapMessageFolderPairs.DefaultIfEmpty(),
                                              (y, z) => new
                                              {
                                                  NewImapMessageUid = y.NewImapMessageUid,
                                                  OldImapMessageFolderPair = z
                                              })
                                  .Where(x => x.OldImapMessageFolderPair == default)
                                  .Select(x => x.NewImapMessageUid)
                                  .ToList();

                return newUids;

            }
        }

        public void AddFolderMessagePairs(IList<EmailMessageFolderPair> emailMessageFolderPairs)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                context.EmailMessageFolderPairs.AddRange(emailMessageFolderPairs);
                context.SaveChanges();
            }
        }
    }
}