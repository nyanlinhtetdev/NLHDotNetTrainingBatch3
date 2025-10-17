using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLHDotNetTrainingBatch3.ProductnSaleProject
{
    internal class ProductnSaleDapperService
    {
        SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = "LAPTOP-7EKI2OO3\\SQLEXPRESS",
            InitialCatalog = "ProductnSale",
            UserID = "mylogin",
            Password = "nyan123",
            TrustServerCertificate = true
        };

        public void ReadProducts()
        {
            using (IDbConnection db = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                db.Open();
                string query = "select * from Tbl_Product where DeleteFlag = 0";
                var lst = db.Query<ProductDTO>(query).ToList();
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
        }

        public void CreateProducts(string ProductName, int Quantity, decimal Price)
        {
            using (IDbConnection db = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                db.Open();
                string query = @"INSERT INTO [dbo].[Tbl_Product]
           ([ProductName]
           ,[Quantity]
           ,[Price]
           ,[DeleteFlag]
           ,[CreatedDateTime])
     VALUES(@ProductName, @Quantity, @Price, 0, @DateTime)";

                int rowAffected = db.Execute(query, new
                {
                    ProductName = ProductName,
                    Quantity = Quantity,
                    Price = Price,
                    DateTime = DateTime.Now
                });
                string message = rowAffected > 0 ? "Successfully created" : "Fail to create";
                Console.WriteLine(message);
            }
        }

        public void UpdateProducts(int ProductId, string ProductName, int Quantity, decimal Price)
        {
            using (IDbConnection db = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                db.Open();
                string query = @"UPDATE [dbo].[Tbl_Product]
   SET ProductName = @ProductName, Quantity = @Quantity, Price = @Price, ModifiedDateTime = @ModifiedDateTime where ProductId = @ProductId";

                int rowAffected = db.Execute(query, new
                {
                    ProductId = ProductId,
                    ProductName = ProductName,
                    Quantity = Quantity,
                    Price = Price,
                    ModifiedDateTime = DateTime.Now
                });
                string message = rowAffected > 0 ? "Successfully updated" : "Fail to update";
                Console.WriteLine(message);
            }
        }

        public void DeleteProducts(int ProductId)
        {
            using (IDbConnection db = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                db.Open();
                string query = @"UPDATE [dbo].[Tbl_Product]
   SET DeleteFlag = 1, ModifiedDateTime = @ModifiedDateTime where ProductId = @ProductId";

                int rowAffected = db.Execute(query, new
                {
                    ProductId = ProductId,
                    ModifiedDateTime = DateTime.Now

                });

                string message = rowAffected > 0 ? "Successfully deleted" : "Fail to delete";
                Console.WriteLine(message);
            }
        }

        public void ReadSale()
        {
            using (IDbConnection db = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                db.Open();
                string query = "select * from Tbl_Sale";
                var lst = db.Query<SaleDTO>(query).ToList();
                foreach (var item in lst)
                {
                    Console.WriteLine(item.SaleId);
                    Console.WriteLine(item.ProductId);
                    Console.WriteLine(item.Quantity);
                    Console.WriteLine(item.Price);
                    Console.WriteLine(item.CreatedDateTime);
                }
            }
        }

        public void CreateSale(int ProductId, int Quantity)
        {
            using (IDbConnection db = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                db.Open();
                
                string productInfoQuery = @"select * from Tbl_Product where ProductId = @ProductId";
                var item = db.Query<SaleDTO>(productInfoQuery, new
                {
                    ProductId = ProductId
                }).FirstOrDefault();
                if(item is null)
                {
                    Console.WriteLine("Id not found");
                    return;
                }
                if(item.Quantity < Quantity)
                {
                    Console.WriteLine("There is no sufficient quantity");
                    return;
                }

                string decrementQuantityQuery = "update Tbl_Product set Quantity = Quantity - @Quantity where ProductId = @ProductId";
                int productRowAffected = db.Execute(decrementQuantityQuery, new
                {
                    Quantity = Quantity,
                    ProductId = ProductId
                });

                if(productRowAffected == 0)
                {
                    Console.WriteLine("Failed");
                    return;
                }

                string createSaleQuery = @"INSERT INTO [dbo].[Tbl_Sale]
           ([ProductId]
           ,[Quantity]
           ,[Price]
           ,[CreatedDateTime])
     VALUES(@ProductId, @Quantity, @Price, @CreatedDateTime)";

                int saleRowAffected = db.Execute(createSaleQuery, new
                {
                    ProductId = ProductId,
                    Quantity = Quantity,
                    Price = item.Price,
                    CreatedDateTime = DateTime.Now
                });
                string message = saleRowAffected > 0 ? "Successfully created" : "Fail to create";
                Console.WriteLine(message);
            }
        }


        class ProductDTO
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public bool DeleteFlag { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public DateTime? ModifiedDateTime { get; set; }
        }

        class SaleDTO
        {
            public int SaleId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public DateTime CreatedDateTime { get; set; }
        }
    }
}