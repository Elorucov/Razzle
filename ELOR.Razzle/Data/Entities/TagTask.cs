namespace ELOR.Razzle.Data.Entities
{
    public sealed class TagTask
    {
        public uint TagId { get; set; }
        public uint TaskId { get; set; }

        public Tag Tag { get; set; } = null;
        public TaskEntity Task { get; set; } = null;
    }
}
