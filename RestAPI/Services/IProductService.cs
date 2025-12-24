using RestAPI.Controllers;
using RestAPI.Dtos;

namespace RestAPI.Services
{
    public interface IProductService
    {
        ProductResponseDto CreateProduct(ProductRequestDto request);
        ProductResponseDto DeleteProduct(int id);
        ProductResponseDto GetProductById(int id);
        ProductListResponseDto GetProducts(int pageNo, int pageSize);
        ProductResponseDto PatchProduct(int id, ProductRequestDto request);
        ProductResponseDto UpdateProduct(int id, ProductRequestDto request);
    }
}
