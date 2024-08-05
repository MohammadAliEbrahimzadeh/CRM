

namespace CRM.Models.Models;

public partial class Product : BaseEntity<int>
{
    public string? Name { get; set; }
}

public partial class Product
{
    public ICollection<Sale>? Sales { get; set; } = new HashSet<Sale>();
}
