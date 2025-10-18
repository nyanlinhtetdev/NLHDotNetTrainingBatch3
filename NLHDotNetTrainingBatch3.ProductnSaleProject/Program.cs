// See https://aka.ms/new-console-template for more information

using NLHDotNetTrainingBatch3.ProductnSaleProject;

//ProductnSaleDapperService dapperService = new ProductnSaleDapperService();
//dapperService.CreateProducts("Air Fryer", 50, 1000000);

ProductnSaleEFCoreService efcoreService = new ProductnSaleEFCoreService();
efcoreService.CreateSales(5, 100);