using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace SignalIRServerTest.Models
{
    public partial class School
    {
        public School()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdCity { get; set; }
        public string Address { get; set; }
        public int IdEducationType { get; set; }

        public virtual City IdCityNavigation { get; set; }
        public virtual EducationType IdEducationTypeNavigation { get; set; }
        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }
}
