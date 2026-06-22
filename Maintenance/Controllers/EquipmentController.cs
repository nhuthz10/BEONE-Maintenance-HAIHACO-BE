using AutoMapper;
using Maintenance.Entities.Equipment;
using Maintenance.Entities.Responses;
using Maintenance.UseCase.EquipmentUseCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maintenance.Controllers
{
    [Route("api/b1cs/v1/equipment")]
    public class EquipmentController : Controller
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IMapper _mapper;

        public EquipmentController(IEquipmentService equipmentService, IMapper mapper)
        {
            _equipmentService = equipmentService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("getAllEquipment")]
        public async Task<IActionResult> GetAllEquipment()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var result = await _equipmentService.GetAllEquipment();

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<List<EquipmentViewModel>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpGet("getAllLine")]
        public async Task<IActionResult> GetAllLine()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var result = await _equipmentService.GetAllLine();

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<List<LineViewModel>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }

        [Authorize]
        [HttpGet("getAllSparePart")]
        public async Task<IActionResult> GetAllSparePart(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ResponseApi<string>.Error(ErrorCode.ValidationFailed, "Invalid data"));
                }

                var result = await _equipmentService.GetAllSparePart(id);

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<List<EquipmentViewModel>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }
    }
}
