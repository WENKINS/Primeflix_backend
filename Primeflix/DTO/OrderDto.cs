namespace Primeflix.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public UserWithoutPasswordDto User { get; set; }
        public DateTime Date { get; set; }
        public float Total { get; set; }
        public ICollection<OrderDetailsDto> Details { get; set; }
    }
}
