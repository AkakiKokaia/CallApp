using CallApp.Application.DTOs.Account;
using CallApp.Application.Features.Account.Commands;
using CallApp.Application.Wrappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CallApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(Response<LoginResponse?>), 200)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost(nameof(Register))]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        public async Task<IActionResult> Register(RegisterAsyncCommand request) => Ok(await Mediator.Send(request));
    }
}
