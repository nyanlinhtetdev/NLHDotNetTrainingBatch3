using DotNetTrainingBatch3.MVCDatabase.AppDbContextModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Services;
using RestAPI.Controllers;
using RestAPI.Dtos;
using static RestAPI.Enums.Enum;

namespace DotNetTrainingBatch3.MVC.Controllers;

public class ProductController : Controller
{
    private readonly ProductDbContext _db;
    public ProductController(ProductDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await _db.TblProducts.AsNoTracking().Where(x => x.DeleteFlag == false).OrderByDescending(x => x.ProductId).ToListAsync();
        return View(products);
    }

    //Shows the create form
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Saves the new product data
    [HttpPost]
    public async Task<IActionResult> Save(TblProduct product)
    {
        product.DeleteFlag = false;
        product.CreatedDateTime = DateTime.Now;
        await _db.TblProducts.AddAsync(product);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.TblProducts.FirstOrDefaultAsync(x => x.ProductId == id);
        if (product is null)
        {
            return RedirectToAction("Index");
        }
        return View(product);
    }

    // Updates the product data
    [HttpPost]
    public async Task<IActionResult> Update(int id, TblProduct product)
    {
        var item = await _db.TblProducts.FirstOrDefaultAsync(x => x.ProductId == id);
        if (item is null)
        {
            return RedirectToAction("Index");
        }

        item.ProductName = product.ProductName;
        item.Quantity = product.Quantity;
        item.Price = product.Price;
        item.ModifiedDateTime = DateTime.Now;

        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.TblProducts.FirstOrDefaultAsync(x => x.ProductId == id);
        if (item is null)
        {
            return RedirectToAction("Index");
        }

        _db.TblProducts.Remove(item);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
