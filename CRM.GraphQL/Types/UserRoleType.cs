using HotChocolate.Types;
using CRM.Models.Models;
using CRM.Models;

namespace CRM.GraphQL.Types;

public class UserRoleType : ObjectType<UserRole>
{
    protected override void Configure(IObjectTypeDescriptor<UserRole> descriptor)
    {
        descriptor.Field(u => u.IsDeleted).Ignore();
        descriptor.Field(u => u.UpdatedAt).Ignore();
        descriptor.Field(u => u.RoleId).Ignore();
        descriptor.Field(u => u.UserId).Ignore();
        descriptor.Field(u => u.User).Ignore();
    }
}
