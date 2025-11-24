using System;
using System.Collections.Generic;

namespace NLHDotNetTrainingBatch3.ProductnSaleDatabase.AppDbContextModels;

public partial class TblProduct
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public int? CategoryId { get; set; }

    public virtual ProductCategory? Category { get; set; }
}
