using static RestAPI.Enums.Enum;

namespace RestAPI.Dtos
{
    public class ProductListResponseDto
    {
        public EnumResultType Type { get; set; }
        public string Message { get; set; }
        public List<ProductDto>? Products { get; set; }
    }
}
