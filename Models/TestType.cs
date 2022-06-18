using System.Collections.Generic;
using System.Text.Json.Serialization;
using WebApplication1;

namespace SignalIRServerTest.Models
{
    public class TestType
    {
        public TestType()
        {
            this.TestPage = new HashSet<TestPage>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
    
        [JsonIgnore]
        public virtual ICollection<TestPage> TestPage { get; set; }
    }
}