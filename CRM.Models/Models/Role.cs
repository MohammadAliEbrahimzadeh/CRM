
namespace CRM.Models.Models;

public partial class Role : BaseEntity<int>
{
    public string? Name { get; set; }
}

public partial class Role
{
    public virtual ICollection<UserRole>? UserRoles { get; set; } = new HashSet<UserRole>();
}
