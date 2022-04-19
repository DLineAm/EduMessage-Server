using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace SignalIRServerTest.Models
{
    public partial class Group
    {
        public Group()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int? IdFaculty { get; set; }
        public int IdSpeciality { get; set; }

        public virtual Faculty IdFacultyNavigation { get; set; }
        public virtual Speciality IdSpecialityNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }
}
