using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using WebApplication1;

#nullable disable

namespace SignalIRServerTest.Models
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
        public int? IdMainCourse { get; set; }
        public int? IdTeacher { get; set; }
        public int? IdTask { get; set; }
        public int? Position{ get; set; }
        public int? IdTestFrame { get; set; }

        public virtual CourseTask IdCourseTaskNavigation { get; set; }
        public virtual MainCourse IdMainCourseNavigation { get; set; }
        public virtual User IdTeacherNavigation { get; set; }
        public virtual TestFrame IdTestFrameNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<CourseAttachment> CourseAttachments { get; set; }
    }
}
