using AutoMapper;
using Maintenance.Dto.Notification;
using Maintenance.Entities.Notification;
using Maintenance.Entities.Responses;
using Maintenance.UseCase.NotificationUseCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maintenance.Controllers
{
    [Route("api/b1cs/v1/notification")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NotificationController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost("createOrUpdateDeviceToken")]
        public async Task<IActionResult> CreateOrUpdateDeviceToken([FromBody] CreateOrUpdateDeviceTokenDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<CreateOrUpdateDeviceTokenModel>(model);

                var accountId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                newModel.AccountId = accountId;

                var result = await _notificationService.CreateOrUpdateDeviceToken(newModel);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<string>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }
    }
}
