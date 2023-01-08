namespace Primeflix.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public UserLessDetailsDto User { get; set; }
        public DateTime Date { get; set; }
        public float Total { get; set; }
        public StatusDto Status { get; set; }
        public ICollection<OrderDetailsDto> Details { get; set; }
    }
}
