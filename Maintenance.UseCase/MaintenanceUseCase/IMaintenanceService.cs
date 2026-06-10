using Maintenance.Entities.Maintenance;
using Maintenance.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.MaintenanceUseCase
{
    public interface IMaintenanceService
    {
        public Task<OperationResult<AllMaintenanceViewModel>> GetAllMaintenance(FindMaintenanceCreterias maintenanceCreterias);
        public Task<OperationResult<AllMaintenanceViewModel>> GetAllMaintenanceTechnical(FindMaintenanceTechnicalCreterias maintenanceCreterias);
        public Task<OperationResult<MaintenanceViewModel>> GetMaintenanceDetail(int id);
        public Task<OperationResult<string>> CreateMaintenance(CreateMaintenanceModel model);
    }
}
