using ELOR.Razzle.Data.Enums;

namespace ELOR.Razzle.DTO.Requests
{
    public class TagAddRequest
    {
        public string Name { get; set; }
        public TagType? Type { get; set; } // nullable is required, otherwise the "NotEmpty" validator and "EnumModelBinder" won't work correctly together!
    }
}
