using Microsoft.AspNetCore.Mvc;
using RestAPI.Dtos;
using RestAPI.Services;
using static RestAPI.Enums.Enum;

namespace NLH.MVC.Controllers
{
    public class ProductSubmitController : Controller
    {
        private readonly IProductService _productService;

        public ProductSubmitController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            int pageNo = 1;
            int pageSize = 100;
            var result = _productService.GetProducts(pageNo, pageSize);
            return View(result);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(ProductRequestDto requestDto)
        {
            var result = _productService.CreateProduct(requestDto);
            TempData["Message"] = result.Message;
            TempData["IsSuccess"] = result.IsSuccess;
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var result = _productService.GetProductById(id);
            if(result.Type == EnumResultType.NotFound)
            {
                TempData["Message"] = result.Message;
                TempData["IsSuccess"] = result.IsSuccess;
                return RedirectToAction("Index");
            }
            return View(result.Product);
        }

        [HttpPost]
        public IActionResult Update(int id, ProductRequestDto requestDto)
        {
            var result = _productService.UpdateProduct(id, requestDto);
            TempData["Message"] = result.Message;
            TempData["IsSuccess"] = result.IsSuccess;
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var result = _productService.DeleteProduct(id);
            TempData["Message"] = result.Message;
            TempData["IsSuccess"] = result.IsSuccess;
            return RedirectToAction("Index");
        }
    }
}
