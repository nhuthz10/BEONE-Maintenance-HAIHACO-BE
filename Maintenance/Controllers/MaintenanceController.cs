using AutoMapper;
using Maintenance.Dto.Maintenance;
using Maintenance.Entities.Maintenance;
using Maintenance.Entities.Responses;
using Maintenance.UseCase.MaintenanceUseCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maintenance.Controllers
{
    [Route("api/b1cs/v1/maintenance")]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IMapper _mapper;

        public MaintenanceController(IMaintenanceService maintenanceService, IMapper mapper)
        {
            _maintenanceService = maintenanceService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("getAllMaintenance")]
        public async Task<IActionResult> GetAllMaintenance([FromQuery] FindMaintenanceDto findCreterias)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<FindMaintenanceCreterias>(findCreterias);

                newModel.AccountId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _maintenanceService.GetAllMaintenance(newModel);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<AllMaintenanceViewModel>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpGet("getAllMaintenanceTechnical")]
        public async Task<IActionResult> GetAllMaintenanceTechnical([FromQuery] FindMaintenanceTechnicalDto findCreterias)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<FindMaintenanceTechnicalCreterias>(findCreterias);

                newModel.AccountId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _maintenanceService.GetAllMaintenanceTechnical(newModel);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<AllMaintenanceViewModel>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpGet("getMaintenance")]
        public async Task<IActionResult> GetMaintenance(int id)
        {
            try
            {
                var result = await _maintenanceService.GetMaintenanceDetail(id);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<MaintenanceViewModel>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpGet("getTrackingPrMaintenance")]
        public async Task<IActionResult> GetTrackingPrMaintenance(int id)
        {
            try
            {
                var result = await _maintenanceService.GetTrackingPrMaintenance(id);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<TrackingPrViewModel>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpPost("createMaintenance")]
        public async Task<IActionResult> CreateMaintenance([FromForm] CreateMaintenanceDto  model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<CreateMaintenanceModel>(model);

                newModel.CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _maintenanceService.CreateMaintenance(newModel);

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

        [Authorize]
        [HttpPost("updateStatusMaintenance")]
        public async Task<IActionResult> UpdateStatusMaintenance([FromForm] UpdateMaintenanceStatusDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<UpdateMaintenanceStatusModel>(model);

                newModel.AccountId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _maintenanceService.UpdateStatusMaintenance(newModel);

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

        [Authorize]
        [HttpPost("createItemRequest")]
        public async Task<IActionResult> CreateItemRequest([FromBody] CreateItemRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<CreateItemRequestModel>(model);

                newModel.AccountId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _maintenanceService.CreateItemRequest(newModel);

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

        [Authorize]
        [HttpPost("createGoodReceipt")]
        public async Task<IActionResult> CreateGoodReceipt([FromBody] CreateGoodReceiptDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<CreateGoodReceiptModel>(model);

                newModel.AccountId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _maintenanceService.CreateRecoveryReceipt(newModel);

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

        [Authorize]
        [HttpPost("createPurchaseRequest")]
        public async Task<IActionResult> CreatePurchaseRequest([FromBody] CreatePurchaseRequestDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<CreatePurchaseRequestModel>(model);

                newModel.AccountId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _maintenanceService.CreatePurchaseRequest(newModel);

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

        [Authorize]
        [HttpPost("createPurchaseRequestService")]
        public async Task<IActionResult> CreatePurchaseRequestService([FromBody] CreatePurchaseRequestServiceDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<CreatePurchaseRequestServiceModel>(model);

                newModel.AccountId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _maintenanceService.CreatePurchaseRequestService(newModel);

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

        [Authorize]
        [HttpPost("saveCheckList")]
        public async Task<IActionResult> SaveCheckList([FromBody] SaveCheckListDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var newModel = _mapper.Map<SaveCheckListModel>(model);

                var result = await _maintenanceService.SaveCheckList(newModel);

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
