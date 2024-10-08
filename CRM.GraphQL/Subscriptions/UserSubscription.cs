﻿using HotChocolate;
using HotChocolate.Types;
using CRM.Models.Models;
using CRM.Common.DTOs.Authentication;

namespace CRM.GraphQL.Subscriptions;

public class UserSubscription
{
    [Subscribe]
    [Topic("OnUserCreated")]
    public UserDto OnUserCreated([EventMessage] UserDto user) => user;
}
