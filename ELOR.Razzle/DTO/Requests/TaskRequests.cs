namespace ELOR.Razzle.DTO.Requests
{
    public class TaskCreateRequest : IIdempotentRequest
    {
        public string Name { get; set; }
        public List<uint> TagIds { get; set; } = new List<uint>();

        public int RandomId { get; set; }
    }
}
