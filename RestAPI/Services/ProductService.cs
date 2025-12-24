using Azure.Core;
using Microsoft.EntityFrameworkCore;
using NLHDotNetTrainingBatch3.ProductnSaleDatabase.AppDbContextModels;
using RestAPI.Controllers;
using RestAPI.Dtos;
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

    public ProductListResponseDto GetProducts(int pageNo, int pageSize)
    {
        ProductListResponseDto dto;
        if (pageNo == 0 && pageSize == 0)
        {
            dto = new ProductListResponseDto
            {
                Type = EnumResultType.ValidationError,
                Message = "Page Number and Page Size must not be 0"
            };
            return dto;
        }
 
        var lst = _db.TblProducts
                .AsNoTracking()
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

        dto = new ProductListResponseDto
        {
            Type = EnumResultType.Success,
            Message = "Success",
            Products = products
        };
        return dto;
    }

    public ProductResponseDto GetProductById(int id)
    {
        ProductResponseDto dto;
        var item = _db.TblProducts.Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductResponseDto
            {
                Type = EnumResultType.NotFound,
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
        dto = new ProductResponseDto
        {
            Type = EnumResultType.Success,
            Message = "Success",
            Product = product
        };
        return dto;
    }

    public ProductResponseDto CreateProduct(ProductRequestDto request)
    {
        ProductResponseDto dto;
        var ProductName = request.ProductName;
        var Price = request.Price;
        var Quantity = request.Quantity;
        if(string.IsNullOrWhiteSpace(ProductName) && Price is null && Quantity is null)
        {
            dto = new ProductResponseDto
            {
                Type = EnumResultType.ValidationError,
                Message = "There is no input."
            };
        }
        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            dto = new ProductResponseDto
            {
                Message = "ProductName is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Price == null)
        {
            dto = new ProductResponseDto
            {
                Message = "Price is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Price.Value < 0)
        {
            dto = new ProductResponseDto
            {
                Message = "Price is negative.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Quantity == null)
        {
            dto = new ProductResponseDto
            {
                Message = "Quantity is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Quantity.Value < 0)
        {
            dto = new ProductResponseDto
            {
                Message = "Quantity is negative.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        var newProductEntity = new TblProduct
        {
            CreatedDateTime = DateTime.Now,
            Price = request.Price.Value,
            DeleteFlag = false,
            ProductName = request.ProductName,
            Quantity = request.Quantity.Value,
        };
        _db.TblProducts.Add(newProductEntity);
        int result = _db.SaveChanges();
        if (result > 0)
        {
            ProductDto product = new ProductDto
            {
                ProductId = newProductEntity.ProductId,
                ProductName = request.ProductName,
                Quantity = request.Quantity.Value,
                Price = request.Price.Value
            };
            dto = new ProductResponseDto
            {
                Message = "Successfully created.",
                Type = EnumResultType.Success,
                Product = product
            };
            return dto;
        }
        dto = new ProductResponseDto
        {
            Message = "Fail to create.",
            Type = EnumResultType.SystemError
        };
        return dto;
    }

    public ProductResponseDto UpdateProduct(int id, ProductRequestDto request)
    {
        ProductResponseDto dto;
        var ProductName = request.ProductName;
        var Price = request.Price;
        var Quantity = request.Quantity;
        if (string.IsNullOrWhiteSpace(ProductName) && Price is null && Quantity is null)
        {
            dto = new ProductResponseDto
            {
                Type = EnumResultType.ValidationError,
                Message = "There is no input."
            };
        }
        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            dto = new ProductResponseDto
            {
                Message = "ProductName is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Price == null)
        {
            dto = new ProductResponseDto
            {
                Message = "Price is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Price.Value < 0)
        {
            dto = new ProductResponseDto
            {
                Message = "Price is negative.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Quantity == null)
        {
            dto = new ProductResponseDto
            {
                Message = "Quantity is missing.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        if (request.Quantity.Value < 0)
        {
            dto = new ProductResponseDto
            {
                Message = "Quantity is negative.",
                Type = EnumResultType.ValidationError
            };
            return dto;
        }
        var item = _db.TblProducts.Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductResponseDto
            {
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
            var product = GetProductById(item.ProductId);
            dto = new ProductResponseDto
            {
                Message = "Successfully updated.",
                Type = EnumResultType.Success,
                Product = product.Product
            };
            return dto;
        }
        dto = new ProductResponseDto
        {
            Message = "Fail to update.",
            Type = EnumResultType.SystemError
        };
        return dto;
    }

    public ProductResponseDto PatchProduct(int id, ProductRequestDto request)
    {
        ProductResponseDto dto;
        var item = _db.TblProducts.FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductResponseDto
            {
                Message = "Id not found",
                Type = EnumResultType.NotFound
            };
            return dto;
        }

        if (string.IsNullOrEmpty(request.ProductName) && request.Price is null && request.Quantity is null)
        {
            dto = new ProductResponseDto
            {
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
                return new ProductResponseDto {
                    Message = "Price is negative",
                    Type = EnumResultType.ValidationError
                };
            }
            item.Price = request.Price.Value;
        }

        // 3. Update Quantity ONLY if provided (not null)
        if (request.Quantity != null)
        {
            if (request.Quantity < 0)
            {
                return new ProductResponseDto
                {
                    Message = "Price is negative",
                    Type = EnumResultType.ValidationError
                };
            }
            item.Quantity = request.Quantity.Value;
        }

        item.ModifiedDateTime = DateTime.Now;
        int result = _db.SaveChanges();
        if (result > 0)
        {
            var product = GetProductById(item.ProductId);
            dto = new ProductResponseDto
            {
                Message = "Successfully updated.",
                Type = EnumResultType.Success,
                Product = product.Product
            };
            return dto;
        }
        dto = new ProductResponseDto
        {
            Message = "Fail to update.",
            Type = EnumResultType.SystemError
        };
        return dto;
    }

    public ProductResponseDto DeleteProduct(int id)
    {
        ProductResponseDto dto;
        var item = _db.TblProducts.Where(x => x.DeleteFlag == false).FirstOrDefault(x => x.ProductId == id);
        if (item is null)
        {
            dto = new ProductResponseDto
            {
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
            dto = new ProductResponseDto
            {
                Message = "Successfully deleted.",
                Type = EnumResultType.Success
            };
            return dto;
        }
        dto = new ProductResponseDto
        {
            Message = "Fail to delete.",
            Type = EnumResultType.SystemError
        };
        return dto;
    }
}
