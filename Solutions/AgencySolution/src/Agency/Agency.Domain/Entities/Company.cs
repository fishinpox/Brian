using Agency.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Agency.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public CompanyCategory Category { get; private set; }
    public string? Website { get; private set; }

    private Company() { }

    public static Company Create(string name, CompanyCategory category, string? website)
    {
        return new Company
        {
            Name = name,
            Category = category,
            Website = website
        };
    }

    public void Update(string name, CompanyCategory category, string? website)
    {
        Name = name;
        Category = category;
        Website = website;
    }
}
