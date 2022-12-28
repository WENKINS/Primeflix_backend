namespace Primeflix.DTO
{
    public class CartDto
    {
        public int Id { get; set; }
        public UserWithoutPasswordDto User { get; set; }
        public ICollection<CartProductWithTitleDto> Products { get; set; }
    }
}
