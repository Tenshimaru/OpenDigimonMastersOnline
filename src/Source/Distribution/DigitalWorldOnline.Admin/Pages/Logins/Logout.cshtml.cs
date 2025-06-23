using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Logins
{
    public class LogoutPageModel : PageModel
    {
        public async Task OnGetAsync()
        {
            await HttpContext
               .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Redirect("/");
        }
    }
}
