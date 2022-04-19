﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace SignalIRServerTest.Models
{
    public partial class City
    {
        public City()
        {
            Schools = new HashSet<School>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<School> Schools { get; set; }
        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }
}
