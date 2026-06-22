using Maintenance.Entities.Equipment;
using Maintenance.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.EquipmentUseCase
{
    public interface IEquipmentService
    {
        public Task<OperationResult<List<EquipmentViewModel>>> GetAllEquipment();
        public Task<OperationResult<List<EquipmentViewModel>>> GetAllSparePart(int id);
        public Task<OperationResult<List<LineViewModel>>> GetAllLine();
    }
}
