using CRM.GraphQL.Queries;

public class Query
{
    public UserQueries UserQueries { get; }
    public SaleQueries SaleQueries { get; }

    public Query(UserQueries userQueries, SaleQueries saleQueries)
    {
        UserQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        SaleQueries = saleQueries ?? throw new ArgumentNullException(nameof(saleQueries));
    }
}
