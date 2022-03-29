using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace SignalIRServerTest
{
    public partial class CourseAttachment : IEquatable<CourseAttachment>
    {
        public int Id { get; set; }
        public int? IdCourse { get; set; }
        public int? IdAttachmanent { get; set; }

        public virtual Attachment IdAttachmanentNavigation { get; set; }
        public virtual Course IdCourseNavigation { get; set; }

        public bool Equals([AllowNull] CourseAttachment other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj);
        }
    }
}
