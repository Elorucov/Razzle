using ELOR.Razzle.Data.Entities;
using ELOR.Razzle.DTO;
using Riok.Mapperly.Abstractions;

namespace ELOR.Razzle.Mappings
{
    // Maps DB entities to API DTOs via Mapperly
    [Mapper]
    public sealed partial class RazzleMapper
    {
        public partial TagDTO ToDto(Tag entity);
        public partial List<TagDTO> ToDto(List<Tag> entities);
    }
}
