using MailKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailSenderAspNetMvc.Models.Domains
{
    public class EmailFolderComparer : IComparer<EmailFolder>
    {
        public int Compare(EmailFolder x, EmailFolder y)
        {
            if (x.Attributes.HasFlag(FolderAttributes.Inbox) && !y.Attributes.HasFlag(FolderAttributes.Inbox))
                return -1;

            if (!x.Attributes.HasFlag(FolderAttributes.Inbox) && y.Attributes.HasFlag(FolderAttributes.Inbox))
                return 1;

            if(x.Attributes.HasFlag(FolderAttributes.Flagged) && !y.Attributes.HasFlag(FolderAttributes.Flagged))
                return -1;

            if (!x.Attributes.HasFlag(FolderAttributes.Flagged) && y.Attributes.HasFlag(FolderAttributes.Flagged))
                return 1;

            if (x.Attributes.HasFlag(FolderAttributes.Important) && !y.Attributes.HasFlag(FolderAttributes.Important))
                return -1;

            if (!x.Attributes.HasFlag(FolderAttributes.Important) && y.Attributes.HasFlag(FolderAttributes.Important))
                return 1;

            if (x.Attributes.HasFlag(FolderAttributes.Sent) && !y.Attributes.HasFlag(FolderAttributes.Sent))
                return -1;

            if (!x.Attributes.HasFlag(FolderAttributes.Sent) && y.Attributes.HasFlag(FolderAttributes.Sent))
                return 1;

            if (x.Attributes.HasFlag(FolderAttributes.Drafts) && !y.Attributes.HasFlag(FolderAttributes.Drafts))
                return -1;

            if (!x.Attributes.HasFlag(FolderAttributes.Drafts) && y.Attributes.HasFlag(FolderAttributes.Drafts))
                return 1;

            if (x.Attributes.HasFlag(FolderAttributes.All) && !y.Attributes.HasFlag(FolderAttributes.All))
                return -1;

            if (!x.Attributes.HasFlag(FolderAttributes.All) && y.Attributes.HasFlag(FolderAttributes.All))
                return 1;

            if (x.Attributes.HasFlag(FolderAttributes.Junk) && !y.Attributes.HasFlag(FolderAttributes.Junk))
                return -1;

            if (!x.Attributes.HasFlag(FolderAttributes.Junk) && y.Attributes.HasFlag(FolderAttributes.Junk))
                return 1;

            if (x.Attributes.HasFlag(FolderAttributes.Trash) && !y.Attributes.HasFlag(FolderAttributes.Trash))
                return -1;

            if (!x.Attributes.HasFlag(FolderAttributes.Trash) && y.Attributes.HasFlag(FolderAttributes.Trash))
                return 1;

            StringComparer sc = StringComparer.CurrentCulture;

            return sc.Compare(x.FullName, y.FullName);
        }

        
    }
}