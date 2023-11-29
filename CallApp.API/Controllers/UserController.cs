using CallApp.Application.DTOs.Account;
using CallApp.Application.Features.Account.Commands;
using CallApp.Application.Features.Account.Queries;
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
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(LoginAsyncCommand request) => Ok(await Mediator.Send(request));
        [AllowAnonymous]
        [HttpPost(nameof(Register))]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        public async Task<IActionResult> Register(RegisterAsyncCommand request) => Ok(await Mediator.Send(request));

        [Authorize]
        [HttpGet(nameof(GetUser))]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        public async Task<IActionResult> GetUser([FromQuery]GetUserByIdAsyncQuery request) => Ok(await Mediator.Send(request));

        [Authorize]
        [HttpGet(nameof(GetAllUsers))]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersAsyncQuery request) => Ok(await Mediator.Send(request));

        [Authorize]
        [HttpDelete(nameof(DeleteUser))]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        public async Task<IActionResult> DeleteUser([FromQuery] DeleteUserAsyncCommand request) => Ok(await Mediator.Send(request));

        [AllowAnonymous]
        [HttpPut(nameof(UpdateUser))]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserAsyncCommand request) => Ok(await Mediator.Send(request));
    }
}
