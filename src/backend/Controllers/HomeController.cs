using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace demoidp.Controllers;

[ApiController]
[AllowAnonymous]
[Route("/")]
public class HomeController : ControllerBase
{
    private readonly Settings _settings;

    public HomeController(IOptionsSnapshot<Settings> settingsOptions)
    {
        _settings = settingsOptions.Value;
    }
    [HttpGet]
    public IActionResult Index()
    {
        if (!User.IsAuthenticated())
        {
            return Redirect("connect/authenticate");
        }

        return LiquidContentResult.Get(DotLiquidTemplates.Home, new
        {
            Username = User.GetDisplayName(),
            CdnUrl = _settings.CdnUrl.TrimEnd('/')
        });
    }
}