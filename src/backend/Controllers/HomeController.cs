using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace demoidp.Controllers;

[ApiController]
[AllowAnonymous]
[Route("/")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        if (!User.IsAuthenticated())
        {
            return Redirect("connect/authenticate");
        }

        return LiquidContentResult.Get(DotLiquidTemplates.Home, new
        {
            Username = User.GetDisplayName()
        });
    }
}