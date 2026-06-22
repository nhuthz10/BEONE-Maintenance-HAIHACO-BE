using Equipment.Infrastructure.SqlServer.Repositories.Equipment;
using Maintenance.Entities.Equipment;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Repositories.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.EquipmentUseCase
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public EquipmentService(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public async Task<OperationResult<List<EquipmentViewModel>>> GetAllEquipment()
        {
            try
            {
                var result = await _equipmentRepository.GetAllEquipment();
                return result;
            }   
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<List<EquipmentViewModel>>> GetAllSparePart(int id)
        {
            try
            {
                var result = await _equipmentRepository.GetAllSparePart(id);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<List<LineViewModel>>> GetAllLine()
        {
            try
            {
                var result = await _equipmentRepository.GetAllLine();
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
