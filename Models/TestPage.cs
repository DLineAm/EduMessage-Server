using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SignalIRServerTest.Models;

#nullable disable

namespace WebApplication1
{
    public partial class TestPage
    {
        public TestPage()
        {
            TestVariants = new HashSet<TestVariant>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int? IdTestFrame { get; set; }
        public int IdTestType { get; set; }

        [JsonIgnore]
        public virtual TestFrame IdTestFrameNavigation { get; set; }
        public virtual TestType IdTestTypeNavigation { get; set; }
        public virtual ICollection<TestVariant> TestVariants { get; set; }
    }
}
