using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication1
{
    public partial class TestVariant
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool? IsCorrect { get; set; }
        public int? IdTestPage { get; set; }

        [JsonIgnore]
        public virtual TestPage IdTestPageNavigation { get; set; }
    }
}
