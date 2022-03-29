

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace SignalIRServerTest
{
    public partial class Course
    {
        public Course()
        {
            CourseAttachments = new HashSet<CourseAttachment>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? IdSpeciality { get; set; }

        public virtual Speciality IdSpecialityNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<CourseAttachment> CourseAttachments { get; set; }
    }
}
