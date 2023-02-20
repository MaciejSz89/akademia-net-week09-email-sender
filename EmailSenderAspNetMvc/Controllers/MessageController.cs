using EmailSenderAspNetMvc.Models.Domains;
using EmailSenderAspNetMvc.Models.Repositories;
using EmailSenderAspNetMvc.Models.ViewModels;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmailSenderAspNetMvc.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {

        EmailConfigurationRepository _emailConfigurationRepository = new EmailConfigurationRepository();
        EmailAddressRepository _emailAddressRepository = new EmailAddressRepository();
        EmailMessageRepository _emailMessageRepository = new EmailMessageRepository();
        EmailAttachmentRepository _emailAttachmentRepository = new EmailAttachmentRepository();
        EmailFolderRepository _emailFolderRepository = new EmailFolderRepository();
        EmailMessageFolderPairRepository _emailMessageFolderPairRepository = new EmailMessageFolderPairRepository();

        public ActionResult CreateMessage(int emailMessageId = 0, int emailFolderId = 0)
        {
            if (Session["EmailConfigurationId"] == null || (int)Session["EmailConfigurationId"] <= 0)
                RedirectToAction("SelectConfiguration", "Configuration");

            var userId = User.Identity.GetUserId();


            var emailConfigurationId = (int)Session["EmailConfigurationId"];


            var emailMessage = emailMessageId == 0 ? GetNewMessage(userId, emailConfigurationId)
                                      : _emailMessageRepository.GetMessage(emailMessageId, userId);

            var configurations = _emailConfigurationRepository
                                    .GetEmailConfigurations(userId);

            var vm = PrepareSendEmailMessageViewModel(emailMessage, emailFolderId);

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMessage(EmailMessage emailMessage,
                                                List<EmailMessageReceiver> emailMessageReceiversTo,
                                                List<EmailMessageReceiver> emailMessageReceiversCc,
                                                List<EmailMessageReceiver> emailMessageReceiversBcc,
                                                List<HttpPostedFileBase> attachments,
                                                string submitButton,
                                                int emailFolderId = 0)
        {


            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            var userId = User.Identity.GetUserId();
            emailMessage.UserId = userId;

            MergeReceivers(emailMessage.EmailMessageReceivers,
                           emailMessageReceiversTo,
                           emailMessageReceiversCc,
                           emailMessageReceiversBcc,
                           userId);

           

            emailMessage.EmailAttachments = _emailAttachmentRepository
                                                            .GetAttachments(emailMessage.EmailAttachments,
                                                                                        emailMessage.Id,
                                                                                        userId);
            AddNewAttachmentsToEmailMessage(emailMessage, attachments);


            emailMessage.EmailConfiguration = _emailConfigurationRepository.GetEmailConfiguration(emailMessage.EmailConfigurationId,
                                                                                                  userId);

            string statusText = "";

            if (submitButton == "Save")
            {
                SaveMessage(emailMessage);
                statusText = "Wiadomość została zapisana";
            }
            else if (submitButton == "Send")
            {                
                SendMessage(emailMessage);
                statusText = "Wiadomość została wysłana";
            }

            if(emailMessage.Id !=0 )
            {               
                var messageUid = _emailMessageFolderPairRepository.GetImapMessageUid(emailMessage.Id, 
                                                                                     emailFolderId);
                var folderFullName = _emailFolderRepository.GetFolderFullName(emailFolderId, 
                                                                              userId);


                DeleteMessageFromImapServer(emailMessage.EmailConfiguration, 
                                            folderFullName, 
                                            messageUid);
            }
           

            return RedirectToAction("Folders", 
                                    "Mailbox", 
                                    new { message = statusText });
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

        private void SaveMessage(EmailMessage emailMessage)
        {
            MimeMessage message = GetMimeMessage(emailMessage);
            using (var client = PrepareImapClient(emailMessage.EmailConfiguration))
            {               

                var drafts = client.GetFolder(SpecialFolder.Drafts);
                drafts.Open(FolderAccess.ReadWrite);


                var messageUid = drafts.Append(message);
                client.Disconnect(true);
            }
        }

        private void DeleteMessageFromImapServer(EmailConfiguration emailConfiguration, 
                                                 string folderFullName, 
                                                 long messageUid)
        {
            using (var client = PrepareImapClient(emailConfiguration))
            {
                var folder = client.GetFolder(folderFullName);
                folder.Open(FolderAccess.ReadWrite);
                var message = folder.GetMessage(new UniqueId((uint)messageUid));
                folder.AddFlags(new UniqueId((uint)messageUid), MessageFlags.Deleted, true);

                folder.Expunge();
                client.Disconnect(true);
            }
                
        }


        private void SendMessage(EmailMessage emailMessage)
        {
            MimeMessage message = GetMimeMessage(emailMessage);

            using (var client = new SmtpClient())
            {
                client.Connect(emailMessage.EmailConfiguration.SmtpHost,
                               emailMessage.EmailConfiguration.SmtpPort,
                               false);

                client.Authenticate(emailMessage.EmailConfiguration
                                                      .EmailAddress
                                                      .Address,
                                    _emailConfigurationRepository.ReadEmailConfigurationPassword(emailMessage.EmailConfiguration));
                client.Send(message);
                client.Disconnect(true);
            }

        }

        private MimeMessage GetMimeMessage(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(emailMessage.EmailConfiguration.EmailAddress.DisplayName,
                                                emailMessage.EmailConfiguration.EmailAddress.Address);
            message.From.Add(new MailboxAddress(emailMessage.EmailConfiguration.EmailAddress.DisplayName,
                                                emailMessage.EmailConfiguration.EmailAddress.Address));


            emailMessage.EmailMessageReceivers
                              .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.To)
                              .ForEach(x =>
                              {
                                  message.To.Add(new MailboxAddress(x.EmailAddress.DisplayName,
                                                                    x.EmailAddress.Address));
                              });
            emailMessage.EmailMessageReceivers
                              .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.CC)
                              .ForEach(x =>
                              {
                                  message.Cc.Add(new MailboxAddress(x.EmailAddress.DisplayName,
                                                                    x.EmailAddress.Address));
                              });

            emailMessage.EmailMessageReceivers
                              .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.BCC)
                              .ForEach(x =>
                              {
                                  message.Bcc.Add(new MailboxAddress(x.EmailAddress.DisplayName,
                                                                    x.EmailAddress.Address));
                              });

            message.Subject = emailMessage.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = emailMessage.Body;


            if (emailMessage.EmailAttachments.Count > 0)
            {
                for (int i = 0; i < emailMessage.EmailAttachments.Count; i++)
                {
                    builder.Attachments.Add(emailMessage.EmailAttachments
                                                              .ElementAt(i)
                                                              .FileName,
                                            emailMessage.EmailAttachments
                                                              .ElementAt(i)
                                                              .FileStream);
                }
            }

            message.Body = builder.ToMessageBody();
            return message;
        }

        private EmailMessage GetNewMessage(string userId, int emailConfigurationId)
        {
            return new EmailMessage
            {
                UserId = userId,
                EmailConfigurationId = emailConfigurationId,
                IsBodyHtml = true
            };
        }
        private EditEmailMessageViewModel PrepareSendEmailMessageViewModel(EmailMessage emailMessage, int emailFolderId)
        {
            var vm = new EditEmailMessageViewModel
            {
                EmailMessage = emailMessage,
                EmailFolderId = emailFolderId,
                Heading = emailMessage.Id != 0 ? "Edycja wiadomości" : "Tworzenie nowej wiadomości"
            };

            if (emailMessage.Id == 0)
            {
                vm.EmailMessageReceiversTo = new List<EmailMessageReceiver>();
                vm.EmailMessageReceiversCc = new List<EmailMessageReceiver>();
                vm.EmailMessageReceiversBcc = new List<EmailMessageReceiver>();
            }
            else
            {
                var emailMessageReceiversTo = emailMessage.EmailMessageReceivers
                                                    .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.To)
                                                    .ToList();
                vm.EmailMessageReceiversTo = emailMessageReceiversTo != null
                                                ? emailMessageReceiversTo
                                                : new List<EmailMessageReceiver>();

                var emailMessageReceiversCc = emailMessage.EmailMessageReceivers
                                                    .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.CC)
                                                    .ToList();
                vm.EmailMessageReceiversCc = emailMessageReceiversCc != null
                                                ? emailMessageReceiversCc
                                                : new List<EmailMessageReceiver>();

                var emailMessageReceiversBcc = emailMessage.EmailMessageReceivers
                                                    .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.BCC)
                                                    .ToList();
                vm.EmailMessageReceiversBcc = emailMessageReceiversBcc != null
                                                ? emailMessageReceiversBcc
                                                : new List<EmailMessageReceiver>();
            }

            vm.DefinedEmailAddresses = _emailAddressRepository.GetDefinedEmailAddresses(emailMessage.UserId);

            return vm;
        }










        private void MergeReceivers(ICollection<EmailMessageReceiver> emailMessageReceivers,
                                    List<EmailMessageReceiver> emailMessageReceiversTo,
                                    List<EmailMessageReceiver> emailMessageReceiversCc,
                                    List<EmailMessageReceiver> emailMessageReceiversBcc,
                                    string userId)

        {

            if (emailMessageReceiversTo != null)
            {
                foreach (EmailMessageReceiver receiverTo in emailMessageReceiversTo)
                {
                    receiverTo.EmailMessageReceiverType = EmailMessageReceiverType.To;
                    emailMessageReceivers.Add(receiverTo);
                }
            }
            if (emailMessageReceiversCc != null)
            {
                foreach (EmailMessageReceiver receiverCc in emailMessageReceiversCc)
                {
                    receiverCc.EmailMessageReceiverType = EmailMessageReceiverType.CC;
                    emailMessageReceivers.Add(receiverCc);
                }
            }
            if (emailMessageReceiversBcc != null)
            {
                foreach (EmailMessageReceiver receiverBcc in emailMessageReceiversBcc)
                {
                    receiverBcc.EmailMessageReceiverType = EmailMessageReceiverType.BCC;
                    emailMessageReceivers.Add(receiverBcc);
                }
            }

            foreach (var receiver in emailMessageReceivers)
            {
                receiver.UserId = userId;
                receiver.EmailAddress.UserId = userId;
                if (receiver.EmailAddressId != 0 && receiver.EmailAddress == null)
                    receiver.EmailAddress = _emailAddressRepository.GetEmailAddress(receiver.EmailAddressId, userId);
            }

        }




        [HttpPost]
        public ActionResult AddMessageReceiver(int index,
                                               int messageId,
                                               EmailMessageReceiverType emailMessageReceiverType,
                                               int emailAddressId = 0)
        {

            var vm = PrepareAddEmailMessageReceiverViewModel(index,
                                                             messageId,
                                                             emailMessageReceiverType,
                                                             emailAddressId);
            return PartialView("_MessageReceiverRow", vm);

        }

        private AddEmailMessageReceiverViewModel PrepareAddEmailMessageReceiverViewModel(int index,
                                                                                         int messageId,
                                                                                         EmailMessageReceiverType receiverType,
                                                                                         int emailAddressId)
        {
            var userId = User.Identity.GetUserId();
            var vm = new AddEmailMessageReceiverViewModel();
            vm.ItemIndex = index;
            vm.MessageId = messageId;
            vm.ReceiverType = receiverType;
            if (emailAddressId > 0)
                vm.EmailAddress = _emailAddressRepository.GetEmailAddress(emailAddressId, userId);
            else
                vm.EmailAddress = new EmailAddress
                {
                    Id = 0,
                    UserId = userId,
                    Address = "",
                    DisplayName = ""
                };
            return vm;
        }



        private void AddNewAttachmentsToEmailMessage(EmailMessage emailMessage, List<HttpPostedFileBase> attachments)
        {
            if (attachments != null)
                attachments.ForEach((x) =>
                {
                    emailMessage.EmailAttachments.Add(new EmailAttachment
                    {
                        UserId = emailMessage.UserId,
                        EmailMessageId = emailMessage.Id,
                        FileName = x.FileName,
                        FileStream = GetDataFromStream(x.InputStream)
                    });
                });


        }

        public byte[] GetDataFromStream(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }


        public ActionResult AddAttachment(int index)
        {
            ViewBag.Index = index;

            return PartialView("_AttachmentRow");

        }


        


    }
}