using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLHDotNetTrainingBatch3.ProductnSaleProject
{
    internal class ProductnSaleADODotNETService
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
            using (SqlConnection conn = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Tbl_Product WHERE DeleteFlag = 0";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(reader["ProductId"]);
                    Console.WriteLine(reader["ProductName"]);
                    Console.WriteLine(reader["Quantity"]);
                    Console.WriteLine(reader["Price"]);
                    Console.WriteLine(reader["CreatedDateTime"]);
                    Console.WriteLine(reader["ModifiedDateTime"]);
                }

                reader.Close();
            }
        }

        public void CreateProducts(string ProductName, int Quantity, decimal Price)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                conn.Open();
                string query = @"INSERT INTO [dbo].[Tbl_Product]
                                 ([ProductName], [Quantity], [Price], [DeleteFlag], [CreatedDateTime])
                                 VALUES (@ProductName, @Quantity, @Price, 0, @DateTime)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductName", ProductName);
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                cmd.Parameters.AddWithValue("@Price", Price);
                cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);

                int rowAffected = cmd.ExecuteNonQuery();
                Console.WriteLine(rowAffected > 0 ? "Successfully created" : "Fail to create");
            }
        }

        public void UpdateProducts(int ProductId, string ProductName, int Quantity, decimal Price)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                conn.Open();
                string query = @"UPDATE [dbo].[Tbl_Product]
                                 SET ProductName = @ProductName, 
                                     Quantity = @Quantity, 
                                     Price = @Price, 
                                     ModifiedDateTime = @ModifiedDateTime
                                 WHERE ProductId = @ProductId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", ProductId);
                cmd.Parameters.AddWithValue("@ProductName", ProductName);
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                cmd.Parameters.AddWithValue("@Price", Price);
                cmd.Parameters.AddWithValue("@ModifiedDateTime", DateTime.Now);

                int rowAffected = cmd.ExecuteNonQuery();
                Console.WriteLine(rowAffected > 0 ? "Successfully updated" : "Fail to update");
            }
        }

        public void DeleteProducts(int ProductId)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                conn.Open();
                string query = @"UPDATE [dbo].[Tbl_Product]
                                 SET DeleteFlag = 1, 
                                     ModifiedDateTime = @ModifiedDateTime 
                                 WHERE ProductId = @ProductId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", ProductId);
                cmd.Parameters.AddWithValue("@ModifiedDateTime", DateTime.Now);

                int rowAffected = cmd.ExecuteNonQuery();
                Console.WriteLine(rowAffected > 0 ? "Successfully deleted" : "Fail to delete");
            }
        }

        public void ReadSale()
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Tbl_Sale";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(reader["SaleId"]);
                    Console.WriteLine(reader["ProductId"]);
                    Console.WriteLine(reader["Quantity"]);
                    Console.WriteLine(reader["Price"]);
                    Console.WriteLine(reader["CreatedDateTime"]);
                }

                reader.Close();
            }
        }

        public void CreateSale(int ProductId, int Quantity)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                conn.Open();

                // Step 1: Get product info
                string productInfoQuery = "SELECT Quantity, Price FROM Tbl_Product WHERE ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(productInfoQuery, conn);
                cmd.Parameters.AddWithValue("@ProductId", ProductId);

                SqlDataReader reader = cmd.ExecuteReader();

                int availableQty = 0;
                decimal price = 0;

                if (reader.Read())
                {
                    availableQty = Convert.ToInt32(reader["Quantity"]);
                    price = Convert.ToDecimal(reader["Price"]);
                }
                else
                {
                    Console.WriteLine("Id not found");
                    reader.Close();
                    return;
                }

                reader.Close();

                if (availableQty < Quantity)
                {
                    Console.WriteLine("There is no sufficient quantity");
                    return;
                }

                // Step 2: Decrease quantity
                string decrementQuantityQuery = "UPDATE Tbl_Product SET Quantity = Quantity - @Quantity WHERE ProductId = @ProductId";
                SqlCommand decCmd = new SqlCommand(decrementQuantityQuery, conn);
                decCmd.Parameters.AddWithValue("@Quantity", Quantity);
                decCmd.Parameters.AddWithValue("@ProductId", ProductId);
                int productRowAffected = decCmd.ExecuteNonQuery();

                if (productRowAffected == 0)
                {
                    Console.WriteLine("Failed");
                    return;
                }

                // Step 3: Insert Sale
                string createSaleQuery = @"INSERT INTO [dbo].[Tbl_Sale]
                                           ([ProductId], [Quantity], [Price], [CreatedDateTime])
                                           VALUES (@ProductId, @Quantity, @Price, @CreatedDateTime)";

                SqlCommand saleCmd = new SqlCommand(createSaleQuery, conn);
                saleCmd.Parameters.AddWithValue("@ProductId", ProductId);
                saleCmd.Parameters.AddWithValue("@Quantity", Quantity);
                saleCmd.Parameters.AddWithValue("@Price", price);
                saleCmd.Parameters.AddWithValue("@CreatedDateTime", DateTime.Now);

                int saleRowAffected = saleCmd.ExecuteNonQuery();
                Console.WriteLine(saleRowAffected > 0 ? "Successfully created" : "Fail to create");
            }
        }
    }
}
