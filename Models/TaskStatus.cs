using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SignalIRServerTest.Models
{
    public class TaskStatus
    {
        public TaskStatus()
        {
            this.CourseAttachment = new HashSet<CourseAttachment>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<CourseAttachment> CourseAttachment { get; set; }
    }
}