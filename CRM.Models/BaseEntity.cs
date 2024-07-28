

namespace CRM.Models;

public class BaseEntity<T>
{
    public BaseEntity()
    {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
        IsDeleted = false;
    }

    public T? Id { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
