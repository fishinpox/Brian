using Agency.Domain.Enums;
using Shared.Infrastructure.Common;

namespace Agency.Domain.Entities;

public class Contact : BaseAuditableEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Title { get; private set; }
    public ContactCategory Category { get; private set; }
    public Guid? CompanyId { get; private set; }

    private Contact() { }

    public static Contact Create(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string? title,
        ContactCategory category,
        Guid? companyId)
    {
        return new Contact
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Title = title,
            Category = category,
            CompanyId = companyId
        };
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string? title,
        ContactCategory category,
        Guid? companyId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Title = title;
        Category = category;
        CompanyId = companyId;
    }
}
