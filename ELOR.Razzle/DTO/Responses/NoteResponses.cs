namespace ELOR.Razzle.DTO.Responses
{
    // TODO: tasks
    public sealed class NotesGetResponse : APIList<NoteDTO>
    {
        public List<TagDTO> Tags { get; init; }
    }
}
