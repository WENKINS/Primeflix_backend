namespace Primeflix.DTO
{
    public class ProductsPageResultsDto
    {
        public List<ProductDto> Products { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
