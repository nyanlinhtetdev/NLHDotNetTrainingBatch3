using RestAPI.Controllers;

namespace RestAPI.Services
{
    public interface IProductService
    {
        ResultDto CreateProduct(ProductRequestDto request);
        ResultDto DeleteProduct(int id);
        ProductGetResponseByIdDto GetProductById(int id);
        ProductGetResponseDto GetProducts(int pageNo, int pageSize);
        ResultDto PatchProduct(int id, ProductRequestDto request);
        ResultDto UpdateProduct(int id, ProductRequestDto request);
    }
}
