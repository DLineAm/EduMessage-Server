using System.Text.Json.Serialization;

using System;
using System.Collections.Generic;

#nullable disable

namespace SignalIRServerTest
{
    public partial class Attachment
    {
        public Attachment()
        {
            CourseAttachments = new HashSet<CourseAttachment>();
            Messages = new HashSet<Message>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int IdType { get; set; }
        public byte[] Data { get; set; }

        public virtual AttachmentType IdTypeNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<CourseAttachment> CourseAttachments { get; set; }
        [JsonIgnore]
        public virtual ICollection<Message> Messages { get; set; }
    }
}
