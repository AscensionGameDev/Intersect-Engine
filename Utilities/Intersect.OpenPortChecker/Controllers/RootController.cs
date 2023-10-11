using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.OpenPortChecker.Controllers;

[Route("")]
public class RootController : Controller
{
    private readonly ILogger<RootController> _logger;
    private readonly PortChecker _portChecker;

    public RootController(ILogger<RootController> logger, PortChecker portChecker)
    {
        _logger = logger;
        _portChecker = portChecker;
    }

    [HttpGet]
    public async Task<IActionResult> CheckPorts([FromHeader] ushort port, [FromQuery] long time)
    {
        var requestTime = DateTime.UtcNow;

        var remoteAddress = Request.HttpContext.Connection.RemoteIpAddress;
        if (remoteAddress == default)
        {
            return Problem();
        }

        var remoteEndPoint = new IPEndPoint(remoteAddress, port);
        var (secret, endPoint) = await _portChecker.CheckPort(remoteEndPoint);
        var responseTime = DateTime.UtcNow;

        var ip = remoteEndPoint.Address.ToString();
        Response.Headers["ip"] = endPoint.ToString();
        Response.Headers["request_time"] = requestTime.ToBinary().ToString();
        Response.Headers["response_time"] = responseTime.ToBinary().ToString();
        Response.Headers["secret"] = secret;
        Response.Headers["time"] = time.ToString();

        _logger.LogDebug("{Ip}, {RequestTime}, {ResponseTime}, {Time}", ip, requestTime, responseTime, time);
        return Ok();
    }
}