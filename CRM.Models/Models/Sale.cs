using CRM.Models.Enums;

namespace CRM.Models.Models;

public partial class Sale : BaseEntity<int>
{
    public decimal PricePerProduct { get; set; }

    public decimal DiscountPerProduct { get; set; }

    public long Count { get; set; }

    public int? UserId { get; set; }

    public int? CompanyId { get; set; }

    public int? ProductId { get; set; }

    public SalesStatus SalesStatus { get; set; }
}


public partial class Sale
{
    public virtual User? User { get; set; }

    public virtual Company? Company { get; set; }

    public virtual Product? Product { get; set; }

}
