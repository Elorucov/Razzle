namespace ELOR.Razzle.DTO
{
    public class APIList<T>
    {
        public int Count { get; set; }
        public List<T> Items { get; set; }
    }
}
