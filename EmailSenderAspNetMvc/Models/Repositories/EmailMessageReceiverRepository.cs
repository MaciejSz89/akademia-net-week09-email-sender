using EmailSenderAspNetMvc.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Repositories
{
    public class EmailMessageReceiverRepository
    {
        EmailAddressRepository _emailAddressRepository = new EmailAddressRepository();
        public void UpdateEmailMessageReceivers(int emailMessageId, string userId, ICollection<EmailMessageReceiver> emailMessageReceivers)
        {

            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var messageReceiversFromDb = context.EmailMessageReceivers.Where(x => x.UserId == userId
                                                                              && x.EmailMessageId == emailMessageId)
                                                                      .ToList();

                var receiversToDelete = messageReceiversFromDb.GroupJoin(emailMessageReceivers,
                                                                     x => x.Id,
                                                                     y => y.Id,
                                                                     (x, y) => new { OldReceiver = x, NewReceivers = y })
                                                          .SelectMany(x => x.NewReceivers.DefaultIfEmpty(),
                                                                     (y, z) => new { OldReceiver = y.OldReceiver, NewReceiver = z })
                                                          .Where(x => x.NewReceiver == null)
                                                          .Select(x => x.OldReceiver)
                                                          .ToList();
                receiversToDelete.ForEach(x =>
                {
                    context.EmailMessageReceivers.Remove(x);
                });

                var receiversToAdd = emailMessageReceivers.GroupJoin(messageReceiversFromDb,
                                                                     x => x.Id,
                                                                     y => y.Id,
                                                                     (x, y) => new { NewReceiver = x, OldReceivers = y })
                                                          .SelectMany(x => x.OldReceivers.DefaultIfEmpty(),
                                                                     (y, z) => new { NewReceiver = y.NewReceiver, OldReceiver = z })
                                                          .Where(x => x.OldReceiver == null)
                                                          .Select(x => x.NewReceiver)
                                                          .ToList();

                receiversToAdd.ForEach(x =>
                {
                    context.EmailMessageReceivers.Add(x);
                });

                var receiversToUpdate = emailMessageReceivers.GroupJoin(messageReceiversFromDb,
                                                                     x => x.Id,
                                                                     y => y.Id,
                                                                     (x, y) => new { NewReceiver = x, OldReceivers = y })
                                                          .SelectMany(x => x.OldReceivers.DefaultIfEmpty(),
                                                                     (y, z) => new { NewReceiver = y.NewReceiver, OldReceiver = z })
                                                          .Where(x => x.OldReceiver != null)
                                                          .Select(x => x)
                                                          .ToList();

                receiversToUpdate.ForEach(x =>
                {
                    if (x.NewReceiver.EmailAddress.Id == 0)
                    {
                        x.OldReceiver.EmailAddressId = 0;
                        x.OldReceiver.EmailAddress = new EmailAddress
                        {
                            UserId = x.NewReceiver.UserId,
                            Address = x.NewReceiver.EmailAddress.Address,
                            DisplayName = x.NewReceiver.EmailAddress.DisplayName,
                            IsDefined = false
                        };
                    }
                    else if (x.NewReceiver.EmailAddress.IsDefined)
                    {
                        x.OldReceiver.EmailAddressId = _emailAddressRepository.GetDefinedEmailAddress(x.NewReceiver.EmailAddressId,
                                                                                                      x.NewReceiver.UserId)
                                                                              .Id;
                        x.OldReceiver.EmailAddress = null;
                    }
                    else
                    {
                        _emailAddressRepository.UpdateNotDefinedEmailAddress(x.NewReceiver.EmailAddress);
                    }
                });






                context.SaveChanges();
            }
        }

    }
}