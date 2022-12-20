namespace Primeflix.DTO
{
    public class CartDto
    {
        public int Id { get; set; }
        public UserWithoutPasswordDto User { get; set; }
        public ICollection<ProductDto> Products { get; set; }
    }
}
