using EmailSenderAspNetMvc.Models.Domains;
using EmailSenderAspNetMvc.Models.Repositories;
using EmailSenderAspNetMvc.Models.ViewModels;
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
        EmailMessageAttachmentRepository _emailMessageAttachmentRepository = new EmailMessageAttachmentRepository();

        public ActionResult CreateMessage(int id = 0)
        {
            var userId = User.Identity.GetUserId();

            var message = id == 0 ? GetNewMessage(userId)
                                  : _emailMessageRepository.GetEmailMessage(id, userId);

            var configurations = _emailConfigurationRepository
                                    .GetEmailConfigurations(userId);

            var vm = PrepareSendEmailMessageViewModel(message, configurations);

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMessage(EmailMessage emailMessage,
                                                      List<EmailMessageReceiver> emailMessageReceiversTo,
                                                      List<EmailMessageReceiver> emailMessageReceiversCc,
                                                      List<EmailMessageReceiver> emailMessageReceiversBcc,
                                                      List<HttpPostedFileBase> attachments,
                                                      bool send,
                                                      bool save)
        {

            var userId = User.Identity.GetUserId();
            emailMessage.UserId = userId;

            MergeReceivers(emailMessage.EmailMessageReceivers,
                           emailMessageReceiversTo,
                           emailMessageReceiversCc,
                           emailMessageReceiversBcc,
                           userId);

            emailMessage.EmailAttachments = _emailMessageAttachmentRepository
                                                    .GetEmailMessageAttachments(emailMessage.EmailAttachments,
                                                                                emailMessage.Id,
            userId);
            AddNewAttachmentsToEmailMessage(emailMessage, attachments);

            if (send)
            {
                try
                {
                    emailMessage.EmailConfiguration = _emailConfigurationRepository.GetEmailConfiguration(emailMessage.EmailConfigurationId, userId);


                    SendEmail(emailMessage);

                    if (emailMessage.Id != 0)
                        _emailMessageRepository.DeleteEmailMessage(emailMessage.Id, userId);

                    ViewBag.MessageStatusText = "Wiadomość została wysłana";

                    return View("MessageStatus");
                }
                catch (Exception)
                {
                    ViewBag.MessageStatusText = "Nie udało się wysłać wiadomości";

                    return View("MessageStatus");
                }
            }

            if (save)
            {

                try
                {
                    if (emailMessage.Id == 0)
                        _emailMessageRepository.AddEmailMessage(emailMessage);
                    else
                    {
                        _emailMessageRepository.UpdateEmailMessage(emailMessage);
                    }
                    ViewBag.MessageStatusText = "Wiadomość została zapisana";

                    return View("MessageStatus");
                }
                catch (Exception)
                {
                    ViewBag.MessageStatusText = "Nie udało sie zapisać wiadomości";

                    return View("MessageStatus");
                }

            }

            return RedirectToAction("Index");
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

        private EmailMessage GetNewMessage(string userId)
        {
            return new EmailMessage
            {
                UserId = userId
            };
        }
        private EditEmailMessageViewModel PrepareSendEmailMessageViewModel(EmailMessage message,
                                                                           List<EmailConfiguration> configurations)
        {
            var vm = new EditEmailMessageViewModel
            {
                EmailMessage = message,
                EmailConfigurations = configurations,
                Heading = message.Id != 0 ? "Edycja wiadomości" : "Tworzenie nowej wiadomości"
            };

            if (message.Id == 0)
            {
                vm.EmailMessageReceiversTo = new List<EmailMessageReceiver>();
                vm.EmailMessageReceiversCc = new List<EmailMessageReceiver>();
                vm.EmailMessageReceiversBcc = new List<EmailMessageReceiver>();
            }
            else
            {
                var emailMessageReceiversTo = message.EmailMessageReceivers
                                                    .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.To)
                                                    .ToList();
                vm.EmailMessageReceiversTo = emailMessageReceiversTo != null
                                                ? emailMessageReceiversTo
                                                : new List<EmailMessageReceiver>();

                var emailMessageReceiversCc = message.EmailMessageReceivers
                                                    .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.CC)
                                                    .ToList();
                vm.EmailMessageReceiversCc = emailMessageReceiversCc != null
                                                ? emailMessageReceiversCc
                                                : new List<EmailMessageReceiver>();

                var emailMessageReceiversBcc = message.EmailMessageReceivers
                                                    .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.BCC)
                                                    .ToList();
                vm.EmailMessageReceiversBcc = emailMessageReceiversBcc != null
                ? emailMessageReceiversBcc
                                                : new List<EmailMessageReceiver>();
            }

            vm.DefinedEmailAddresses = _emailAddressRepository.GetDefinedEmailAddresses(message.UserId);

            return vm;
        }

        public ActionResult DraftMessages()
        {

            var userId = User.Identity.GetUserId();

            List<EmailMessage> messages = _emailMessageRepository.GetDraftEmailMessages(userId);

            return View(messages);
        }








        private void MergeReceivers(ICollection<EmailMessageReceiver> emailMessageReceivers,
                                    List<EmailMessageReceiver> emailMessageReceiversTo,
                                    List<EmailMessageReceiver> emailMessageReceiversCc,
                                    List<EmailMessageReceiver> emailMessageReceiversBcc,
                                    string userId)

        {
            if (emailMessageReceiversTo == null)
                emailMessageReceiversTo = new List<EmailMessageReceiver>();

            if (emailMessageReceiversCc == null)
                emailMessageReceiversCc = new List<EmailMessageReceiver>();

            if (emailMessageReceiversBcc == null)
                emailMessageReceiversBcc = new List<EmailMessageReceiver>();

            foreach (EmailMessageReceiver receiverTo in emailMessageReceiversTo)
            {
                receiverTo.EmailMessageReceiverType = EmailMessageReceiverType.To;
                emailMessageReceivers.Add(receiverTo);
            }
            foreach (EmailMessageReceiver receiverCc in emailMessageReceiversCc)
            {
                receiverCc.EmailMessageReceiverType = EmailMessageReceiverType.CC;
                emailMessageReceivers.Add(receiverCc);
            }
            foreach (EmailMessageReceiver receiverBcc in emailMessageReceiversBcc)
            {
                receiverBcc.EmailMessageReceiverType = EmailMessageReceiverType.BCC;
                emailMessageReceivers.Add(receiverBcc);
            }

            foreach (var receiver in emailMessageReceivers)
            {
                receiver.UserId = userId;
                receiver.EmailAddress.UserId = userId;
                if (receiver.EmailAddressId != 0 && receiver.EmailAddress == null)
                    receiver.EmailAddress = _emailAddressRepository.GetEmailAddress(receiver.EmailAddressId, userId);
            }
        }

        private void SendEmail(EmailMessage emailMessage)
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
                            message.To.Add(new MailboxAddress(x.EmailAddress.DisplayName,
                                                              x.EmailAddress.Address));
                        });

            emailMessage.EmailMessageReceivers
                        .Where(x => x.EmailMessageReceiverType == EmailMessageReceiverType.BCC)
                        .ForEach(x =>
                        {
                            message.To.Add(new MailboxAddress(x.EmailAddress.DisplayName,
                                                              x.EmailAddress.Address));
                        });

            message.Subject = emailMessage.Subject;
            var builder = new BodyBuilder();
            builder.TextBody = emailMessage.Content;


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

        public ActionResult AddAttachment(int index)
        {
            ViewBag.Index = index;

            return PartialView("_AttachmentRow");

        }


        [HttpPost]
        public ActionResult DeleteMessage(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _emailMessageRepository.DeleteEmailMessage(id, userId);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }


            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult DeleteEmailMessageReceiver(int messageId, int receiverId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _emailMessageRepository.DeleteEmailMessageReceiver(messageId, receiverId, userId);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }


            return Json(new { Success = true });
        }

    }
}