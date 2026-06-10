using AutoMapper;
using Maintenance.Entities.Factory;
using Maintenance.Entities.Responses;
using Maintenance.UseCase.FactoryUseCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers
{
    [Route("api/b1cs/v1/factory")]
    public class FactoryController : Controller
    {
        private readonly IFactoryService _factoryService;
        private readonly IMapper _mapper;

        public FactoryController(IFactoryService factoryService, IMapper mapper)
        {
            _factoryService = factoryService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("getAllFactory")]
        public async Task<IActionResult> GetAllFactory()
        {
            try
            {
                var result = await _factoryService.GetAllFactory();

                if (!result.IsSuccess)
                {
                    return BadRequest(ResponseApi<string>.Error(result.ErrorCode, result.Message));
                }

                return Ok(ResponseApi<List<FactoryViewModel>>.Success(result.Data, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseApi<string>.Error(ErrorCode.InternalServerError, "Error from the server"));
            }
        }
    }
}
