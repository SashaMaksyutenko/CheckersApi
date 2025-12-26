using CheckersApi.Engine;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("/healthz")]
//public class HealthController : ControllerBase
//{
//    private readonly IEngineAdapter _engine;

//    public HealthController(IEngineAdapter engine)
//    {
//        _engine = engine;
//    }

//    [HttpGet]
//    public IActionResult Get([FromServices] ChinookWorkerPool pool)
//    {
//        return Ok(new { ok = true, workers = pool.Count });
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using CheckersApi.Engine;

[ApiController]
[Route("/healthz")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // Try to resolve a worker pool (Chinook or KingsRow) to report real count.
        // If none is registered (e.g., KingsRow DLL only), fall back to 1.
        var chinookPool = HttpContext.RequestServices.GetService<ChinookWorkerPool>();
        var workers = chinookPool?.Count ?? 1;

        return Ok(new { ok = true, workers });
    }
}