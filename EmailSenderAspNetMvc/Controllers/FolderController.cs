using EmailSenderAspNetMvc.Models.Domains;
using EmailSenderAspNetMvc.Models.Repositories;
using EmailSenderAspNetMvc.Models.ViewModels;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using MimeKit;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using System.Web.UI.WebControls.WebParts;

namespace EmailSenderAspNetMvc.Controllers
{
    [Authorize]
    public class FolderController : Controller
    {
        EmailConfigurationRepository _emailConfigurationRepository = new EmailConfigurationRepository();
        EmailFolderRepository _emailFolderRepository = new EmailFolderRepository();
        EmailMessageFolderPairRepository _emailFolderMessagePairRepository = new EmailMessageFolderPairRepository();
        EmailFolderMessageRepository _emailFolderMessageRepository = new EmailFolderMessageRepository();
        EmailAddressRepository _emailAddressRepository = new EmailAddressRepository();
        EmailFolderAttachmentRepository _emailFolderAttachmentRepository = new EmailFolderAttachmentRepository();

        public ActionResult Folders()
        {

            var userId = User.Identity.GetUserId();
            Synchronize(_emailConfigurationRepository.GetEmailConfigurations(userId)[0], userId);

            var model = _emailFolderRepository.GetFolders(_emailConfigurationRepository.GetEmailConfigurations(userId)[0]);

            return View(model);
        }

        private void Synchronize(EmailConfiguration emailConfiguration, string userId)
        {


            var imapClient = PrepareImapClient(emailConfiguration);
            var imapFolders = SynchronizeFolders(imapClient,
                                                 emailConfiguration.Id,
                                                 userId);

            var imapFolderMessageUidsPairs = GetImapFolderUidsPairs(imapFolders);

            DetachOldMessagesFromFolders(imapFolderMessageUidsPairs,
                                         emailConfiguration.Id,
                                         userId);

            var newImapFolderUidsPairs = GetNewFolderUidsPairs(imapFolderMessageUidsPairs,
                                                               emailConfiguration.Id,
                                                               userId);



            var newImapFolderMessagesPairs = SynchronizeMessages(newImapFolderUidsPairs,
                                                                 emailConfiguration.Id,
                                                                 userId);

            SynchronizeNewMesageFolderPairs(newImapFolderMessagesPairs,
                                         emailConfiguration.Id,
                                         userId);

            imapClient.Disconnect(true);

            _emailFolderMessageRepository.DeleteFolderMessagesWithNoFolder(userId);

            _emailAddressRepository.DeleteAllNotReferencedEmailAddresses(userId);

        }

        private IDictionary<IMailFolder, IList<UniqueId>> GetNewFolderUidsPairs(IDictionary<IMailFolder, IList<UniqueId>> imapFolderMessageUidsPairs,
                                                                                   int emailConfigurationId,
                                                                                   string userId)
        {
            var newFolderUidsPairs = new Dictionary<IMailFolder, IList<UniqueId>>();

            foreach (var item in imapFolderMessageUidsPairs)
            {
                var imapFolder = item.Key;
                var uids = item.Value;

                var newUids = _emailFolderMessagePairRepository.GetNewFolderMessageUids(imapFolder.FullName,
                                                                                        uids.Select(x => (long)x.Id).ToList(),
                                                                                        emailConfigurationId,
                                                                                        userId)
                                                               .Select(x => new UniqueId((uint)x))
                                                               .ToList();
                newFolderUidsPairs.Add(imapFolder, newUids);
            }

            return newFolderUidsPairs;


        }

        private void DetachOldMessagesFromFolders(IDictionary<IMailFolder, IList<UniqueId>> imapFolderMessageUidsPairs,
                                                  int emailConfigurationId,
                                                  string userId)
        {
            var newFolderMessagePairsDictionary = imapFolderMessageUidsPairs.ToDictionary(x => x.Key.FullName,
                                                                                          x => (IList<long>)x.Value
                                                                                                             .Select(y => (long)y.Id)
                                                                                                             .ToList());

            _emailFolderMessagePairRepository.DeleteOldFolderMessagePairs(newFolderMessagePairsDictionary,
                                                                          emailConfigurationId,
                                                                          userId);
        }

        private IDictionary<IMailFolder, IList<UniqueId>> GetImapFolderUidsPairs(IList<IMailFolder> imapFolders)
        {
            var imapFolderMessagesDictionary = new Dictionary<IMailFolder, IList<UniqueId>>();

            foreach (var imapFolder in imapFolders)
            {
                if (!imapFolder.Exists)
                    continue;

                imapFolder.Open(FolderAccess.ReadOnly);

                var imapUids = imapFolder.Search(SearchQuery.All)
                                         .ToList();



                imapFolderMessagesDictionary.Add(imapFolder, imapUids);

                imapFolder.Close();


            }

            return imapFolderMessagesDictionary;
        }

        private void SynchronizeNewMesageFolderPairs(IDictionary<IMailFolder, IList<(UniqueId, IMessageSummary)>> newImapFolderMessagesPairs,
                                                     int emailConfigurationId,
                                                     string userId)
        {

            var newEmailMessageFolderPairs = PrepareFolderMassagePairs(newImapFolderMessagesPairs,
                                                                       emailConfigurationId,
                                                                       userId);

            _emailFolderMessagePairRepository.AddFolderMessagePairs(newEmailMessageFolderPairs);

        }

        private IList<EmailMessageFolderPair> PrepareFolderMassagePairs(IDictionary<IMailFolder, IList<(UniqueId, IMessageSummary)>> imapFolderMessagesPairs,
                                                                        int emailConfigurationId,
                                                                        string userId)
        {
            var emailMessageFolderPairs = new List<EmailMessageFolderPair>();

            foreach (var imapFolderMessagesPair in imapFolderMessagesPairs)
            {
                var folderFullName = imapFolderMessagesPair.Key.FullName;
                var folderId = _emailFolderRepository.GetFolderId(folderFullName,
                                                                  emailConfigurationId,
                                                                  userId);

                foreach (var item in imapFolderMessagesPair.Value)
                {
                    var imapFolderMessageUid = (long)item.Item1.Id;
                    var imapMessageId = item.Item2.Envelope.MessageId;

                    var messageId = _emailFolderMessageRepository.GetMessageId(imapMessageId,
                                                                               emailConfigurationId,
                                                                               userId);

                    emailMessageFolderPairs.Add(new EmailMessageFolderPair
                    {
                        UserId = userId,
                        EmailFolderMessageId = messageId,
                        ImapFolderMessageUid = imapFolderMessageUid,
                        EmailFolderId = folderId
                    });
                }
            }

            return emailMessageFolderPairs;
        }

        private IDictionary<IMailFolder, IList<(UniqueId, IMessageSummary)>> SynchronizeMessages(IDictionary<IMailFolder, IList<UniqueId>> imapFolderMessageUidsPairs,
                                                                                     int emailConfigurationId,
                                                                                     string userId)
        {

            var imapFolderMessagesDictionary = new Dictionary<IMailFolder, IList<(UniqueId, IMessageSummary)>>();

            foreach (var pair in imapFolderMessageUidsPairs)
            {
                var imapFolder = pair.Key;

                if (!imapFolder.Exists)
                    continue;

                var imapUids = pair.Value;

                imapFolder.Open(FolderAccess.ReadOnly);



                var imapMessageSummaries = imapFolder.Fetch(imapUids, MessageSummaryItems.UniqueId |
                                                                      MessageSummaryItems.Envelope |
                                                                      MessageSummaryItems.BodyStructure);

                var emailFolderMessagesToAdd = ConvertImapMessageToEmailFolderMessage(imapFolder,
                                                                                      imapMessageSummaries,
                                                                                      emailConfigurationId,
                                                                                      userId);


                _emailFolderMessageRepository.AddFolderMessages(emailFolderMessagesToAdd);


                imapFolderMessagesDictionary.Add(imapFolder,
                                                 imapUids.Zip(imapMessageSummaries,
                                                              (x, y) => (x, y))
                                            .ToList());




                imapFolder.Close();


            }
            return imapFolderMessagesDictionary;
        }


        private IList<IMailFolder> SynchronizeFolders(ImapClient imapClient,
                                              int emailConfigurationId,
                                              string userId)
        {

            var imapFolders = GetFoldersFromImapServer(imapClient);
            var dbFolders = PrepareEmailFolders(emailConfigurationId,
                                                userId,
                                                imapFolders);
            _emailFolderRepository.UpdateFolders(emailConfigurationId,
                                                 userId,
                                                 dbFolders);

            return imapFolders;

        }
        private IList<EmailFolderMessage> ConvertImapMessageToEmailFolderMessage(IMailFolder imapFolder,
                                                                                 IList<IMessageSummary> imapMessageSummmaries,
                                                                                 int emailConfigurationId,
                                                                                 string userId)
        {
            var emailFolderMessages = new List<EmailFolderMessage>();
            imapMessageSummmaries.ForEach(messageSummary =>
            {
                var receivers = new List<EmailFolderMessageReceiver>();

                foreach (var receiverTo in messageSummary.Envelope.To.Mailboxes)
                {
                    receivers.Add(new EmailFolderMessageReceiver
                    {
                        UserId = userId,
                        EmailMessageReceiverType = EmailMessageReceiverType.To,
                        EmailAddress = new EmailAddress
                        {
                            UserId = userId,
                            DisplayName = receiverTo.Name,
                            Address = receiverTo.Address
                        }
                    });
                }

                foreach (var receiverCc in messageSummary.Envelope.Cc.Mailboxes)
                {
                    receivers.Add(new EmailFolderMessageReceiver
                    {
                        UserId = userId,
                        EmailMessageReceiverType = EmailMessageReceiverType.CC,
                        EmailAddress = new EmailAddress
                        {
                            UserId = userId,
                            DisplayName = receiverCc.Name,
                            Address = receiverCc.Address
                        }
                    });
                }

                foreach (var receiverBcc in messageSummary.Envelope.Bcc.Mailboxes)
                {
                    receivers.Add(new EmailFolderMessageReceiver
                    {
                        UserId = userId,
                        EmailMessageReceiverType = EmailMessageReceiverType.BCC,
                        EmailAddress = new EmailAddress
                        {
                            UserId = userId,
                            DisplayName = receiverBcc.Name,
                            Address = receiverBcc.Address
                        }
                    });
                }

                var bodyPart = messageSummary.HtmlBody != null ? messageSummary.HtmlBody : messageSummary.TextBody;

                var isBodyHtml = bodyPart.IsHtml;

                var body = (TextPart)imapFolder.GetBodyPart(messageSummary.UniqueId, bodyPart);
                var bodyText = body.Text;



                var emailFolderMessage = new EmailFolderMessage
                {
                    From = new EmailAddress
                    {
                        UserId = userId,
                        DisplayName = messageSummary.Envelope.From.Mailboxes.FirstOrDefault()?.Name,
                        Address = messageSummary.Envelope.From.Mailboxes.FirstOrDefault()?.Address
                    },
                    EmailConfigurationId = emailConfigurationId,
                    UserId = userId,
                    EmailFolderMessageReceivers = receivers,
                    ImapMessageId = messageSummary.Envelope.MessageId,
                    Subject = messageSummary.Envelope.Subject,
                    Date = messageSummary.Date,
                    Flags = messageSummary.Flags,
                    Body = bodyText,
                    IsBodyHtml = isBodyHtml

                };


                foreach (var attachment in messageSummary.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {

                     

                        var entity = imapFolder.GetBodyPart(messageSummary.UniqueId, attachment);

                        string fileName;

                        if (entity is MimeKit.MessagePart)
                        {
                            var rfc822 = (MimeKit.MessagePart)entity;

                            fileName = attachment.PartSpecifier + ".eml";

                            rfc822.Message.WriteTo(ms);
                        }
                        else
                        {
                            var part = (MimeKit.MimePart)entity;

                            if (part.Content == null)
                                continue;

                            fileName = part.FileName;


                            part.Content.DecodeTo(ms);


                        }



                        emailFolderMessage.EmailFolderAttachments.Add(new EmailFolderAttachment
                        {
                            UserId = userId,
                            FileName = fileName,
                            FileStream = ms.ToArray()
                        });
                    }

                }





                emailFolderMessages.Add(emailFolderMessage);
            });

            return emailFolderMessages;

        }


        private IList<IMailFolder> GetFoldersFromImapServer(ImapClient imapClient)
        {

            var foldersFromServer = imapClient.GetFolders(new MailKit.FolderNamespace(' ', ""), false);

            return foldersFromServer;


        }

        private List<EmailFolder> PrepareEmailFolders(int emailConfigurationId,
                                                      string userId,
                                                      IList<IMailFolder> imapFolders)
        {
            var foldersToDb = new List<EmailFolder>();

            foreach (var folder in imapFolders)
            {

                if (folder.Exists)
                {

                    var emailFolder = new EmailFolder
                    {
                        Name = folder.Attributes.HasFlag(FolderAttributes.Inbox) ? "Odebrane" : folder.Name,
                        Attributes = folder.Attributes,
                        FullName = folder.FullName,
                        EmailConfigurationId = emailConfigurationId,
                        EmailFolders = new List<EmailFolder> { },
                        ParentFolderName = folder.ParentFolder.FullName,
                        UserId = userId

                    };
                    foldersToDb.Add(emailFolder);
                }
            }

            return foldersToDb;
        }

        private ImapClient PrepareImapClient(EmailConfiguration emailConfiguration)
        {
            ImapClient client = new ImapClient();
            client.Connect(emailConfiguration.ImapHost,
                           emailConfiguration.ImapPort,
                           SecureSocketOptions.SslOnConnect);
            client.Authenticate(emailConfiguration
                                            .EmailAddress
                                            .Address,
                                _emailConfigurationRepository.ReadEmailConfigurationPassword(emailConfiguration));

            return client;
        }

        public ActionResult EmailFolderMessages(int folderId)
        {
            var vm = PrepareDisplayEmailFolderMessagesViewModel(folderId);


            return PartialView("FolderMessages", vm);
        }

        public ActionResult EmailFolderMessage(int folderId, int messageId)
        {
            var vm = PrepareDisplayEmailFolderMessageViewModel(folderId, messageId);

            return PartialView("FolderMessage", vm);
        }

        public ActionResult EmailFolderAttachment(int attachmentId)
        {
            var userId = User.Identity.GetUserId();
            var attachment = _emailFolderAttachmentRepository.GetAttachment(attachmentId, userId);

            return new FileContentResult(attachment.FileStream, attachment.FileName);
        }


        private DisplayEmailFolderMessagesViewModel PrepareDisplayEmailFolderMessagesViewModel(int folderId)
        {
            var userId = User.Identity.GetUserId();
            var vm = new DisplayEmailFolderMessagesViewModel
            {
                EmailFolder = _emailFolderRepository.GetFolder(folderId, userId),
                EmailMessageFolderMessages = _emailFolderRepository.GetFolderMessages(folderId, userId)
            };
            return vm;
        }

        private DisplayEmailFolderMessageViewModel PrepareDisplayEmailFolderMessageViewModel(int folderId, int messageId)
        {
            var userId = User.Identity.GetUserId();
            var vm = new DisplayEmailFolderMessageViewModel
            {
                EmailFolder = _emailFolderRepository.GetFolder(folderId, userId),
                EmailFolderMessage = _emailFolderMessageRepository.GetMessage(messageId, userId)
            };
            return vm;
        }





    }
}