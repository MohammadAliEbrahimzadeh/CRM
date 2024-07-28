using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Models.Models;

public partial class UserRole : BaseEntity<int>
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

}

public partial class UserRole
{
    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
