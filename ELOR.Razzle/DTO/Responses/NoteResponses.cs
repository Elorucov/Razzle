namespace ELOR.Razzle.DTO.Responses
{
    public sealed class NotesGetResponse : APIList<NoteDTO>
    {
        public List<TagDTO> Tags { get; init; }
        public List<TaskDTO> Tasks { get; init; }
    }
}
