using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLHDotNetTrainingBatch3.ProductnSaleDatabase.AppDbContextModels;
using RestAPI.Services;
using static RestAPI.Enums.Enum;

namespace RestAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{pageNo}/{pageSize}")]
    public IActionResult GetProducts(int pageNo, int pageSize)
    {
        var result = _productService.GetProducts(pageNo, pageSize);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Products);
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var result = _productService.GetProductById(id);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost]
    public IActionResult CreateProduct(ProductRequestDto request)
    {
        var result = _productService.CreateProduct(request);
        if (result.Type == EnumResultType.ValidationError)
        {
            return BadRequest(result);
        }
        if (result.Type == EnumResultType.NotFound)
        {
            return NotFound(result);
        }
        if (result.Type == EnumResultType.SystemError)
        {
            return StatusCode(500, result);
        }
        return Ok(result);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, ProductRequestDto request)
    {
        var result = _productService.UpdateProduct(id, request);
        if (result.Type == EnumResultType.ValidationError)
        {
            return BadRequest(result);
        }
        if (result.Type == EnumResultType.NotFound)
        {
            return NotFound(result);
        }
        if (result.Type == EnumResultType.SystemError)
        {
            return StatusCode(500, result);
        }
        return Ok(result);
    }

    [HttpPatch("{id}")]
    public IActionResult PatchProduct(int id, ProductRequestDto request)
    {
        var result = _productService.PatchProduct(id, request);
        if (result.Type == EnumResultType.ValidationError)
        {
            return BadRequest(result);
        }
        if (result.Type == EnumResultType.NotFound)
        {
            return NotFound(result);
        }
        if (result.Type == EnumResultType.SystemError)
        {
            return StatusCode(500, result);
        }
        return Ok(result);
    }
    

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var result = _productService.DeleteProduct(id);
        if (result.Type == EnumResultType.ValidationError)
        {
            return BadRequest(result);
        }
        if (result.Type == EnumResultType.NotFound)
        {
            return NotFound(result);
        }
        if (result.Type == EnumResultType.SystemError)
        {
            return StatusCode(500, result);
        }
        return Ok(result);
    }
}

public class ProductRequestDto
{
    public string? ProductName { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }
}

public class ProductGetResponseDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }
}

public class ProductGetListResponseDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }
}