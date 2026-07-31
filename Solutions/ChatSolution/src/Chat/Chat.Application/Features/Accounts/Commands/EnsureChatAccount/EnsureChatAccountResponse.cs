namespace Chat.Application.Features.Accounts.Commands.EnsureChatAccount;

/// <summary>
/// Password is returned in plaintext by design - see the ChatWindow plan's confirmed target UX:
/// the embedded Stoat client has no external-token-login route, so the user manually logs into
/// its own login screen with these credentials once. Idempotent: repeat calls for a profile that
/// already has a linked account return the same stored credentials rather than rotating them.
/// </summary>
public record EnsureChatAccountResponse(string Username, string Email, string Password);
