using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace demoidp.Controllers
{
    [Route("account")]
    public class AccountController: ControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        public AccountController(IIdentityServerInteractionService interaction, IEventService events)
        {
            _interaction = interaction;
            _events = events;
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var user = HttpContext.User;
            if (user.IsAuthenticated())
            {
                await HttpContext.SignOutAsync();

                await _events.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetDisplayName()));
            }

            var logoutContext = await _interaction.GetLogoutContextAsync(logoutId);

            if (logoutContext != null && !string.IsNullOrEmpty(logoutContext.PostLogoutRedirectUri))
            {
                return Redirect(logoutContext.PostLogoutRedirectUri);
            }

            return RedirectToAction("Get", "authenticate");
        }
    }
}
