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
    }
}
