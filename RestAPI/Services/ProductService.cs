using Azure.Core;
using NLHDotNetTrainingBatch3.ProductnSaleDatabase.AppDbContextModels;
using RestAPI.Controllers;
using System.Collections.Generic;
using System.Linq;
using static RestAPI.Enums.Enum;

namespace RestAPI.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public ProductGetResponseDto GetProducts(int pageNo, int pageSize)
    {
        ProductGetResponseDto dto;
        if (pageNo == 0)
        {
            dto = new ProductGetResponseDto
            {
                IsSuccess = false,
                Message = "Page Number is 0"
            };
            return dto;
        }
        if (pageSize == 0)
        {
            dto = new ProductGetResponseDto
            {
                IsSuccess = false,
                Message = "Page Size is 0"
            };
            return dto;
        }
        var lst = _db.TblProducts
                .OrderByDescending(x => x.ProductId)
                .Where(x => x.DeleteFlag == false)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        var products = lst.Select(item => new ProductDto
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            Price = item.Price
        }).ToList();

        dto = new ProductGetResponseDto
        {
            IsSuccess = true,
            Message = "Success",
            Products = products
        };
        return dto;
    }

    public ProductGetResponseByIdDto GetProductById(int id)
    {
        ProductGetResponseByIdDto dto;
        var item = _db.TblProducts.Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductGetResponseByIdDto
            {
                IsSuccess = false,
                Message = "Product not found."
            };
            return dto;
        }

        ProductDto product = new ProductDto
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            Price = item.Price
        };
        dto = new ProductGetResponseByIdDto
        {
            IsSuccess = true,
            Message = "Success",
            Product = product
        };
        return dto;
    }

    public ResultDto CreateProduct(ProductRequestDto request)
    {
        ResultDto dto;
        if (string.IsNullOrEmpty(request.ProductName))
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "ProductName is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Price == null)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Price is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Price.Value < 0)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Price is negative.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Quantity == null)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Quantity is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Quantity.Value < 0)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Quantity is negative.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        _db.TblProducts.Add(new TblProduct
        {
            CreatedDateTime = DateTime.Now,
            Price = request.Price.Value,
            DeleteFlag = false,
            ProductName = request.ProductName,
            Quantity = request.Quantity.Value,
        });
        int result = _db.SaveChanges();
        if (result > 0)
        {
            dto = new ResultDto
            {
                IsSuccess = true,
                Message = "Successfully created.",
                Type = EnumResultType.Success
            };
            return dto;
        }
        dto = new ResultDto
        {
            IsSuccess = false,
            Message = "Fail to create.",
            Type = EnumResultType.SystemError
        };
        return dto;
    }

    public ResultDto UpdateProduct(int id, ProductRequestDto request)
    {
        ResultDto dto;
        if (string.IsNullOrEmpty(request.ProductName))
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "ProductName is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Price == null)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Price is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Price.Value < 0)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Price is negative.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Quantity == null)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Quantity is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Quantity.Value < 0)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Quantity is negative.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        var item = _db.TblProducts.Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Product not found.",
                Type = EnumResultType.NotFound
            };
            return dto;
        }

        item.ProductName = request.ProductName;
        item.Price = request.Price.Value;
        item.Quantity = request.Quantity.Value;
        item.ModifiedDateTime = DateTime.Now;
        int result = _db.SaveChanges();
        if (result > 0)
        {
            dto = new ResultDto
            {
                IsSuccess = true,
                Message = "Successfully updated.",
                Type = EnumResultType.Success
            };
            return dto;
        }
        dto = new ResultDto
        {
            IsSuccess = false,
            Message = "Fail to update.",
            Type = EnumResultType.SystemError
        };
        return dto;
    }

    public ResultDto PatchProduct(int id, ProductRequestDto request)
    {
        ResultDto dto;
        var item = _db.TblProducts.FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Id not found",
                Type = EnumResultType.NotFound
            };
            return dto;
        }

        if (string.IsNullOrEmpty(request.ProductName) && request.Price is null && request.Quantity is null)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "There is nothing to update.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }

        if (!string.IsNullOrEmpty(request.ProductName))
        {
            item.ProductName = request.ProductName;
        }


        if (request.Price != null)
        {
            if (request.Price < 0)
            {
                return new ResultDto { IsSuccess = false, Message = "Price is negative" };
            }
            item.Price = request.Price.Value;
        }

        // 3. Update Quantity ONLY if provided (not null)
        if (request.Quantity != null)
        {
            if (request.Quantity < 0)
            {
                return new ResultDto { IsSuccess = false, Message = "Quantity is negative" };
            }
            item.Quantity = request.Quantity.Value;
        }

        item.ModifiedDateTime = DateTime.Now;
        int result = _db.SaveChanges();
        if (result > 0)
        {
            dto = new ResultDto
            {
                IsSuccess = true,
                Message = "Successfully updated.",
                Type = EnumResultType.Success
            };
            return dto;
        }
        dto = new ResultDto
        {
            IsSuccess = false,
            Message = "Fail to update.",
            Type = EnumResultType.SystemError
        };
        return dto;
    }

    public ResultDto DeleteProduct(int id)
    {
        ResultDto dto;
        var item = _db.TblProducts.Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ResultDto
            {
                IsSuccess = false,
                Message = "Product not found.",
                Type = EnumResultType.NotFound
            };
            return dto;
        }

        item.DeleteFlag = true;
        item.ModifiedDateTime = DateTime.Now;
        int result = _db.SaveChanges();
        if (result > 0)
        {
            dto = new ResultDto
            {
                IsSuccess = true,
                Message = "Successfully deleted.",
                Type = EnumResultType.Success
            };
            return dto;
        }
        dto = new ResultDto
        {
            IsSuccess = false,
            Message = "Fail to delete.",
            Type = EnumResultType.SystemError
        };
        return dto;
    }
}


public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
public class ProductGetResponseDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<ProductDto>? Products { get; set; }
}

public class ProductGetResponseByIdDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public ProductDto? Product { get; set; }
}

public class ResultDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public EnumResultType Type { get; set; }
}
