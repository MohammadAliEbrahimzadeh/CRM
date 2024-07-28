using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Common.DTOs.Authentication;

public class UserDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public List<UserRoleDto>? UserRoles { get; set; }
}
