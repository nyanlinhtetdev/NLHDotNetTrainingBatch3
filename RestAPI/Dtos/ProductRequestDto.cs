using System.ComponentModel.DataAnnotations;

namespace RestAPI.Dtos
{
    public class ProductRequestDto
    {
        public string? ProductName { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }
}
