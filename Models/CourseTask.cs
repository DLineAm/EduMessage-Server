using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SignalIRServerTest.Models
{
    public class CourseTask
    {
        public CourseTask()
        {
            this.Course = new HashSet<Course>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public System.DateTime? EndTime { get; set; }

        [JsonIgnore]
        public virtual ICollection<Course> Course { get; set; }
    }
}