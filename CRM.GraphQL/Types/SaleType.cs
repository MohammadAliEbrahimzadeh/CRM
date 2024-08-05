using HotChocolate.Types;
using CRM.Models.Models;

namespace CRM.GraphQL.Types;

public class SaleType : ObjectType<Sale>
{
    protected override void Configure(IObjectTypeDescriptor<Sale> descriptor)
    {
        descriptor.Field(u => u.UpdatedAt).Ignore();
        descriptor.Field(u => u.IsDeleted).Ignore();
        descriptor.Field(u => u.CompanyId).Ignore();
        descriptor.Field(u => u.UserId).Ignore();
    }
}
