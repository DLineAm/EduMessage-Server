using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace SignalIRServerTest.Models
{
    public partial class Speciality
    {
        public Speciality()
        {
            MainCourse = new HashSet<MainCourse>();
            Groups = new HashSet<Group>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public virtual ICollection<MainCourse> MainCourse { get; set; }
        [JsonIgnore]
        public virtual ICollection<Group> Groups { get; set; }
    }
}
