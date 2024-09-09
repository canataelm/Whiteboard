namespace Api.Controllers;

public class AliveController : BaseApiController
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(DateTime.Now);
    }
}
