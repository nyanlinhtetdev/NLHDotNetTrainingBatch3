using System;
using System.Collections.Generic;

namespace ThemeMvcAppDatabase.AppDbContextModels;

public partial class TblStudent
{
    public int StudentId { get; set; }

    public string StudentNo { get; set; } = null!;

    public string StudentName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public bool DeleteFlag { get; set; }
}
