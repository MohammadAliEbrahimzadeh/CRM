﻿

namespace CRM.Models.Models;

public partial class Company : BaseEntity<int>
{
    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? NationalCode { get; set; }
}

public partial class Company
{
    public ICollection<Sale>? Sales { get; set; } = new HashSet<Sale>();
}


