using AutoMapper;
using Maintenance.Dto.Auth;
using Maintenance.Entities.Auth;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Entities;
using Maintenance.UseCase.AuthUseCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maintenance.Controllers
{
    [Route("api/b1cs/v1/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost("appSignIn")]
        public async Task<IActionResult> AppSignIn([FromBody] LoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<LoginModel>(model);

                var result = await _authService.AppSignIn(newModel);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<ViewLoginModel>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn([FromBody] LoginAdminDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<LoginAdminModel>(model);

                var result = await _authService.SignIn(newModel);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<ViewLoginModel>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<CreateUserModel>(model);

                var result = await _authService.CreateUser(newModel);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<string>.Success(message: result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }


        [Authorize]
        [HttpGet("lockUser/{accoutnId}")]
        public async Task<IActionResult> LockUser([FromRoute] string accoutnId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accoutnId))
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var result = await _authService.LockoutUser(accoutnId);

                if (!result.IsSuccess)
                {
                    var statusCode = (int)result.ErrorCode;
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<string>.Success(message: result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpGet("unLockUser/{accoutnId}")]
        public async Task<IActionResult> UnLockUser([FromRoute] string accoutnId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accoutnId))
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var result = await _authService.UnlockoutUser(accoutnId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<string>.Success(message: result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpGet("tokenExpire")]
        public async Task<IActionResult> TokenExpire()
        {
            try
            {
                var accoutnId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await _authService.CheckLockUser(accoutnId);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                var user = await _userManager.FindByIdAsync(accoutnId);
                await _userManager.UpdateAsync(user);

                return Ok(ResponseApi<string>.Success(message: result.Message));
            }
            catch (Exception) { return BadRequest(); }
        }
    }
}
