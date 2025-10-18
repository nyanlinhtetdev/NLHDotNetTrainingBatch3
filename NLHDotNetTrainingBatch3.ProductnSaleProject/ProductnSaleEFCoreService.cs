using Microsoft.Data.SqlClient;
using NLHDotNetTrainingBatch3.ProductnSaleDatabase.AppDbContextModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLHDotNetTrainingBatch3.ProductnSaleProject
{
    internal class ProductnSaleEFCoreService
    {
        AppDbContext db;

        public ProductnSaleEFCoreService()
        {
            db = new AppDbContext();
        }
        public void ReadProducts()
        {
            var lst = db.TblProducts.ToList();
            foreach (var item in lst)
            {
                Console.WriteLine(item.ProductId);
                Console.WriteLine(item.ProductName);
                Console.WriteLine(item.Quantity);
                Console.WriteLine(item.Price);
                Console.WriteLine(item.CreatedDateTime);
                Console.WriteLine(item.ModifiedDateTime);
            }
        }
        public void CreateProducts(string ProductName, int Quantity, decimal Price)
        {
            db.TblProducts.Add(new TblProduct
            {
                ProductName = ProductName,
                Quantity = Quantity,
                Price = Price,
                CreatedDateTime = DateTime.Now
            });

            int result = db.SaveChanges();
            string message = result > 0 ? "Successfully created" : "Fail to create";
            Console.WriteLine(message);
        }

        public void UpdateProducts(int ProductId, string ProductName, int Quantity, decimal Price)
        {
            var item = db.TblProducts.FirstOrDefault(x => x.ProductId == ProductId);
            if(item is null)
            {
                Console.WriteLine("Id not found");
            }
            item.ProductName = ProductName;
            item.Quantity = Quantity;
            item.Price = Price;
            item.ModifiedDateTime = DateTime.Now;
            int result = db.SaveChanges();
            string message = result > 0 ? "Successfully updated" : "Fail to update";
            Console.WriteLine(message);
        }

        public void DeleteProducts(int ProductId)
        {
            var item = db.TblProducts.FirstOrDefault(x => x.ProductId == ProductId);
            if (item is null)
            {
                Console.WriteLine("Id not found");
            }
            item.DeleteFlag = true;
            item.ModifiedDateTime = DateTime.Now;
            int result = db.SaveChanges();
            string message = result > 0 ? "Successfully deleted" : "Fail to delete";
            Console.WriteLine(message);
        }

        public void ReadSales()
        {
            var lst = db.TblSales.ToList();
            foreach (var item in lst)
            {
                Console.WriteLine( item.SaleId);
                Console.WriteLine(item.ProductId);               
                Console.WriteLine(item.Quantity);
                Console.WriteLine(item.Price);
                Console.WriteLine(item.CreatedDateTime);
            }
        }

        public void CreateSales(int ProductId, int Quantity)
        {
            var item = db.TblProducts.FirstOrDefault(x => x.ProductId == ProductId);
            if(item is null)
            {
                Console.WriteLine( "Id not found");
                return;
            }
            if (item.Quantity < Quantity)
            {
                Console.WriteLine("There is no sufficient quantity");
                return;
            }

            item.Quantity = item.Quantity - Quantity;
            item.ModifiedDateTime = DateTime.Now;

            int productResult = db.SaveChanges();
            if (productResult == 0)
            {
                Console.WriteLine("Failed");
                return;
            }

            db.TblSales.Add(new TblSale
            {
                ProductId = ProductId,
                Quantity = Quantity,
                Price = item.Price,
                CreatedDateTime = DateTime.Now
            });


            int saleResult = db.SaveChanges();
            string message = saleResult > 0 ? "Successfully created" : "Fail to create";
            Console.WriteLine(message);
        }
    }
}
