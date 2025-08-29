using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Waffle.Core.Foundations;

[Authorize]
[Route("api/[controller]")]
public class BaseController : Controller
{
}
