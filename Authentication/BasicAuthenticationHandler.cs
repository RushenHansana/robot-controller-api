using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using robot_controller_api.Persistence;

namespace robot_controller_api.Authentication;
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

protected override Task<AuthenticateResult> HandleAuthenticateAsync()
{
    // Add the WWW-Authenticate header to prompt the client for credentials
    Response.Headers.Add("WWW-Authenticate", @"Basic realm=""Access to the robot controller.""" );

    var authHeader = Request.Headers["Authorization"].ToString();
    if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
    {
        return Task.FromResult(AuthenticateResult.Fail("Missing or invalid Authorization header."));
    }

    try
    {
        var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
        var decodedBytes = Convert.FromBase64String(encodedCredentials);
        var decodedCredentials = Encoding.UTF8.GetString(decodedBytes);

        var parts = decodedCredentials.Split(':', 2);
        if (parts.Length != 2)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header format."));
        }

        var email = parts[0];
        var password = parts[1];

        // Look for the user by email
        var user = UserDataAccess.GetUsers().FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed. User not found."));
        }

        // Use the PasswordHasher to verify the password
        var hasher = new PasswordHasher<UserModel>();
        var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verificationResult != PasswordVerificationResult.Success)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid credentials."));
        }

        // If credentials are valid, create claims for the user
        var claims = new[]
        {
            new Claim("name", $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "User")  // Default role is "User" if null
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));  // Successful authentication
    }
    catch (Exception ex)
    {
        // Log the error for debugging purposes
        Logger.LogError(ex, "Authentication error.");
        return Task.FromResult(AuthenticateResult.Fail("Authentication error."));
    }
}
}

