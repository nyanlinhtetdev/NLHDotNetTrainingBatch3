using static RestAPI.Enums.Enum;

namespace RestAPI.Dtos
{
    public class ProductResponseDto
    {
        public EnumResultType Type { get; set; }
        public string Message { get; set; }
        public ProductDto? Product { get; set; }
    }
}
