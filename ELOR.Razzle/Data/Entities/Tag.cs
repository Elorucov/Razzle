using ELOR.Razzle.Data.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ELOR.Razzle.Data.Entities
{
    public sealed class Tag
    {
        public uint Id { get; set; }
        public TagType Type { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<TagNote> TagNotes { get; set; } = new List<TagNote>();
        public ICollection<TagTask> TagTasks { get; set; } = new List<TagTask>();
    }
}
