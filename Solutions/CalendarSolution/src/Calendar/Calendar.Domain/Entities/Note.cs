using Shared.Infrastructure.Common;

namespace Calendar.Domain.Entities;

public class Note : BaseAuditableEntity
{
    public Guid ProfileId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    private Note() { }

    public static Note Create(Guid profileId, string title, string content)
    {
        return new Note
        {
            ProfileId = profileId,
            Title = title,
            Content = content
        };
    }

    public void Update(string title, string content)
    {
        Title = title;
        Content = content;
    }
}
