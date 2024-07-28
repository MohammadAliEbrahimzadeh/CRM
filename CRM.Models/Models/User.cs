

namespace CRM.Models.Models;

public partial class User : BaseEntity<int>
{
    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

}


public partial class User
{
    public virtual ICollection<UserRole>? UserRoles { get; set; } = new HashSet<UserRole>();
}
