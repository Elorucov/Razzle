using ELOR.Razzle.Data.Entities;
using ELOR.Razzle.DTO;
using Riok.Mapperly.Abstractions;

namespace ELOR.Razzle.Mappings
{
    // Maps DB entities to API DTOs via Mapperly
    [Mapper]
    public sealed partial class RazzleMapper
    {
        [MapperIgnoreSource(nameof(Tag.TagNotes))]
        [MapperIgnoreSource(nameof(Tag.TagTasks))]
        public partial TagDTO ToDto(Tag entity);

        public partial List<TagDTO> ToDto(List<Tag> entities);

        public NoteDTO ToDto(Note entity, bool isCropped = false)
        {
            return new NoteDTO
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Text = entity.Text,
                IsCropped = isCropped,
                TagIds = entity.TagNotes.Select(t => t.TagId).ToList(),
                TaskId = entity.TaskId,
                CashFlow = entity.CashFlow != null && entity.CashFlow.FlowType.HasValue ?
                    ToDto(entity.CashFlow) : null
            };
        }

        public List<NoteDTO> ToDto(List<Note> entities)
        {
            return entities.Select(t => ToDto(t)).ToList();
        }

        public CashFlowDTO ToDto(CashFlow entity)
        {
            return new CashFlowDTO
            {
                FlowType = entity.FlowType.Value,
                Amount = entity.Amount
            };
        }

        [MapperIgnoreSource(nameof(TaskEntity.Flags))]
        [MapperIgnoreSource(nameof(TaskEntity.CompletionNote))]
        [MapperIgnoreSource(nameof(TaskEntity.Notes))]
        [MapperIgnoreSource(nameof(TaskEntity.TagTasks))]
        public partial TaskDTO ToDto(TaskEntity entity);

        public partial List<TaskDTO> ToDto(List<TaskEntity> entities);
    }
}
