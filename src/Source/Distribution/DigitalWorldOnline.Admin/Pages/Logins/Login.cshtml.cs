using DigitalWorldOnline.Admin.Shared;
using DigitalWorldOnline.Commons.ViewModel.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Logins
{
    public class LoginPageModel : PageModel
    {
        private readonly LoginModelRepository _loginRepository;

        public LoginPageModel(LoginModelRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public async Task OnGetAsync(string username, string returnUrl)
        {
            try
            {
                var login = _loginRepository.Logins.FirstOrDefault(x => x.Username == username);

                if (login != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, login.Username),
                        new Claim(ClaimTypes.Role, login.AccessLevel.ToString())
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                           new ClaimsPrincipal(
                               new ClaimsIdentity(
                                    claims,
                                    CookieAuthenticationDefaults.AuthenticationScheme)));

                    _loginRepository.Logins.Remove(login);
                }

                if(!string.IsNullOrEmpty(returnUrl))
                    Response.Redirect($"{returnUrl}");
                else
                    Response.Redirect("/");
            }
            catch
            {
                Response.Redirect("/login");
            }
        }
    }
}
