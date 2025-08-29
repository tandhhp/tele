using Microsoft.AspNetCore.Mvc;
using Waffle.Core.Foundations;
using Waffle.Core.Interfaces.IService;

namespace Waffle.Controllers;

public class TransportController(ITransportService _transportService) : BaseController
{
    [HttpGet("options")]
    public async Task<IActionResult> GetTransportOptions() => Ok(await _transportService.GetTransportOptionsAsync());
}
