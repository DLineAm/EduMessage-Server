using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace SignalIRServerTest
{
    public partial class Speciality
    {
        public Speciality()
        {
            Courses = new HashSet<Course>();
            Groups = new HashSet<Group>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public virtual ICollection<Course> Courses { get; set; }
        [JsonIgnore]
        public virtual ICollection<Group> Groups { get; set; }
    }
}
