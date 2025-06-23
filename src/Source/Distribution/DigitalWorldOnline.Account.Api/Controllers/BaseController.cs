using Microsoft.AspNetCore.Mvc;

namespace DigitalWorldOnline.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected string GetToken()
        {
            return HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault(x => x.StartsWith("Bearer"))?
                .Substring("Bearer ".Length)
                .Trim();
        }
    }
}
