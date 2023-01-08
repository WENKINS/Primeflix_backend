namespace Primeflix.DTO
{
    public class CartDto
    {
        public int Id { get; set; }
        public UserLessDetailsDto User { get; set; }
        public ICollection<CartProductWithTitleDto> Products { get; set; }
    }
}
