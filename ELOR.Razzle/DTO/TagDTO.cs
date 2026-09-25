using ELOR.Razzle.Data.Enums;

namespace ELOR.Razzle.DTO
{
    public sealed class TagDTO
    {
        public uint Id { get; init; }
        public string Name { get; init; }
        public TagType Type { get; init; }
    }
}
