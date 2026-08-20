namespace Creator.Domain.Constants;

public static class ReservedSlugs
{
    public static readonly IReadOnlySet<string> Values = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "api", "hubs", "admin", "www", "app", "auth", "login", "logout", "static", "assets",
        "health", "about", "help", "support", "faq", "contact", "changelog", "terms", "privacy",
        "vtuberhub", "creator", "creators"
    };
}
