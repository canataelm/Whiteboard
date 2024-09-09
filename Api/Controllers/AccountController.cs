using Common.Models.ReturnTypes.Concrete;

namespace Api.Controllers;

public class AccountController : BaseApiController
{

    [HttpPost("login")]
    public IActionResult Login(LoginDto loginDto)
    {
        UserResultDto userResultDto = new ()
        {
            Token = "cemil"
        };


        var result = new SuccessDataResult<UserResultDto>(userResultDto,"cemil");

        return Ok(result);
    }
}
