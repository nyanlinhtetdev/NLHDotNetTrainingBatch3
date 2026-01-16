using Microsoft.AspNetCore.Mvc;
using RestAPI.Dtos;
using RestAPI.Services;
using System.Linq.Expressions;
using static RestAPI.Enums.Enum;

namespace NLH.MVC.Controllers
{
    public class ProductAjaxController : Controller
    {
        private readonly IProductService _productService;

        public ProductAjaxController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {      
            return View();
        }
        [HttpGet]
        public IActionResult List()
        {
            try
            {
                int pageNo = 1;
                int pageSize = 100;
                var result = _productService.GetProducts(pageNo, pageSize);

                if (result.Type == EnumResultType.ValidationError)
                {
                    return BadRequest(result.Message);
                }
                return Json(result.Products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(ProductRequestDto requestDto)
        {
            try
            {
                //throw new Exception("nyan lin htet error");
                var result = _productService.CreateProduct(requestDto);

                if (result.Type == EnumResultType.ValidationError)
                {
                    return BadRequest(result.Message);
                }
                if (result.Type == EnumResultType.NotFound)
                {
                    return NotFound(result.Message);
                }
                if (result.Type == EnumResultType.SystemError)
                {
                    return StatusCode(500, result.Message);
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var result = _productService.GetProductById(id);
                if(result.Type == EnumResultType.NotFound)
                {
                    return NotFound(result.Message);
                }
                return View(result.Product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Update(int id, ProductRequestDto requestDto)
        {
            try {
                var result = _productService.UpdateProduct(id, requestDto);
                if (result.Type == EnumResultType.ValidationError)
                {
                    return BadRequest(result.Message);
                }
                if (result.Type == EnumResultType.NotFound)
                {
                    return NotFound(result.Message);
                }
                if (result.Type == EnumResultType.SystemError)
                {
                    return StatusCode(500, result.Message);
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _productService.DeleteProduct(id);
                if (result.Type == EnumResultType.NotFound)
                {
                    return NotFound(result.Message);
                }
                if (result.Type == EnumResultType.SystemError)
                {
                    return StatusCode(500, result.Message);
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
