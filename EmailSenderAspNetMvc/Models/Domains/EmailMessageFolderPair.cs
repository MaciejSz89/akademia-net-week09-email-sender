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
        public int EmailFolderMessageId { get; set; }

        public string UserId { get; set; }

        public long ImapFolderMessageUid { get; set; }
        public EmailFolder EmailFolder { get; set; }
        public EmailFolderMessage EmailFolderMessage { get; set; }

        public ApplicationUser User { get; set; }

    }
}