namespace ELOR.Razzle.DTO.Requests
{
    public interface IIdempotentRequest
    {
        int RandomId { get; }
    }
}
