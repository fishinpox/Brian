using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser() { Id = Guid.CreateVersion7(); }
}
