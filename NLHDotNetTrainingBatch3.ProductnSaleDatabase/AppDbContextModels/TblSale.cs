using System;
using System.Collections.Generic;

namespace NLHDotNetTrainingBatch3.ProductnSaleDatabase.AppDbContextModels;

public partial class TblSale
{
    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedDateTime { get; set; }
}
