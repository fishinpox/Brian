using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class Memo : BaseEntity
{
    public Guid ProfileId { get; private set; }
    public DateOnly Date { get; private set; }
    public string Text { get; private set; } = string.Empty;

    private Memo() { }

    public static Memo Create(Guid profileId, DateOnly date, string text)
    {
        return new Memo
        {
            ProfileId = profileId,
            Date = date,
            Text = text
        };
    }
}
