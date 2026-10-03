namespace ELOR.Razzle.DTO.Responses
{
    public sealed class TasksGetResponse : APIList<TaskDTO>
    {
        public List<TagDTO> Tags { get; init; }
        public List<NoteDTO> Notes { get; init; }
    }
}
