using MailKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailFolder
    {
        public EmailFolder()
        {
            EmailMessageFolderPairs = new Collection<EmailMessageFolderPair>();
            EmailFolders = new Collection<EmailFolder>();
        }

        public int Id { get; set; }        

        public int EmailConfigurationId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string FullName { get; set; }

        public string ParentFolderName { get; set; }

        

        public int? ParentFolderId { get; set; }

        public EmailFolder ParentFolder { get; set; }

        public FolderAttributes Attributes { get; set; }

        public string UserId { get; set; }

        public ICollection<EmailMessageFolderPair> EmailMessageFolderPairs { get; set; }
        public ICollection<EmailFolder> EmailFolders { get; set; }





    }
}