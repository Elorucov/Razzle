namespace ELOR.Razzle.Data.Entities
{
    public sealed class TagNote
    {
        public uint TagId { get; set; }
        public uint NoteId { get; set; }

        public Tag Tag { get; set; } = null;
        public Note Note { get; set; } = null;
    }
}
