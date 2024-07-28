﻿using HotChocolate.Types;
using CRM.Models.Models;

namespace CRM.GraphQL.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Field(u => u.UpdatedAt).Ignore();
        descriptor.Field(u => u.IsDeleted).Ignore();
        descriptor.Field(u => u.PasswordHash).Ignore();
        descriptor.Field(u => u.PasswordSalt).Ignore();
    }
}