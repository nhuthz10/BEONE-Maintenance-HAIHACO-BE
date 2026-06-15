using Maintenance.Entities.Equipment;
using Maintenance.Entities.Maintenance;
using Maintenance.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Repositories.Maintenance
{
    public interface IMaintenanceRepository
    {
        public Task<OperationResult<AllMaintenanceViewModel>> GetAllMaintenance(FindMaintenanceCreterias maintenanceCreterias);
        public Task<OperationResult<AllMaintenanceViewModel>> GetAllMaintenanceTechnical(FindMaintenanceTechnicalCreterias maintenanceCreterias);
        public Task<OperationResult<MaintenanceViewModel>> GetMaintenanceDetail(int id);
        public Task<OperationResult<string>> CreateMaintenance(CreateMaintenanceModel model);
        public Task<OperationResult<string>> UpdateStatusMaintenance(UpdateMaintenanceStatusModel model);
        public Task<OperationResult<string>> CreateItemRequest(CreateItemRequestModel model);
        public Task<OperationResult<string>> CreatePurchaseRequest(CreatePurchaseRequestModel model);
        public Task<OperationResult<string>> CreatePurchaseRequestService(CreatePurchaseRequestServiceModel model);
        public Task<OperationResult<string>> SaveCheckList(SaveCheckListModel model);
        public Task CreateMaintenancePeriodic();
    }
}
