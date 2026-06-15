using Maintenance.Entities.Equipment;
using Maintenance.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equipment.Infrastructure.SqlServer.Repositories.Equipment
{
    public interface IEquipmentRepository
    {
        public Task<OperationResult<List<EquipmentViewModel>>> GetAllEquipment();
        public Task<OperationResult<List<EquipmentViewModel>>> GetAllSparePart();
    }
}
