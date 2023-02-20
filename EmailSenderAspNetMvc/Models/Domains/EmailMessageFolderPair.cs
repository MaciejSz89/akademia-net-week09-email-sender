using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailMessageFolderPair
    {       

        public int Id { get; set; }
        public int EmailFolderId { get; set; }
        public int EmailMessageId { get; set; }

        public string UserId { get; set; }

        public long ImapMessageUid { get; set; }
        public EmailFolder EmailFolder { get; set; }
        public EmailMessage EmailMessage { get; set; }

        public ApplicationUser User { get; set; }

    }
}