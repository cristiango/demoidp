using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;

namespace demoidp.Controllers;

[ApiController]
[Route("connect/authenticate")]
public class AuthenticateController : ControllerBase
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;

    public AuthenticateController(IIdentityServerInteractionService interaction, IEventService events)
    {
        _interaction = interaction;
        _events = events;
    }

    [HttpGet]
    public IActionResult Get(string? returnUrl) =>
        LiquidContentResult.Get(DotLiquidTemplates.Login, new
        {
            ReturnUrl = returnUrl
        });

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromForm] string username,
        [FromForm] string? password,
        [FromForm] string? roles,
        [FromForm] string returnUrl = "")
    {
        var userSub = username.ToDeterministicGuid().ToString();
        await _events.RaiseAsync(new UserLoginSuccessEvent(username, userSub, username));

        var claims = new List<Claim>
        {
            new(JwtClaimTypes.Name, username),
            new(JwtClaimTypes.Email, $"{username}@company.com"),
            new(JwtClaimTypes.EmailVerified, "false"),
            new(JwtClaimTypes.PreferredUserName, username)
        };
        var roleClaims = (roles ?? "").Split(",").Where(r => !string.IsNullOrEmpty(r))
            .Select(role => new Claim(JwtClaimTypes.Role, role.Trim()));
        claims.AddRange(roleClaims);

        var idsrvUser = new IdentityServerUser(userSub)
        {
            AdditionalClaims = claims.ToArray(),
            DisplayName = username
        };
        await HttpContext.SignInAsync(idsrvUser);

        //bool isValidReturnUrl = _interaction.IsValidReturnUrl(returnUrl);
        bool isLocalUrl = Url.IsLocalUrl(returnUrl);
        if (!string.IsNullOrEmpty(returnUrl) || isLocalUrl)
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}