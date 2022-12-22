using Newtonsoft.Json;

namespace Primeflix.DTO
{
    [JsonObject]
    public class CartProductDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
