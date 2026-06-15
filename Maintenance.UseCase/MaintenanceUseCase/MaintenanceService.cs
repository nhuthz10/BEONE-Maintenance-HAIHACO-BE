using AutoMapper;
using Maintenance.Entities.Maintenance;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Repositories.Maintenance;
using Maintenance.UseCase.EquipmentUseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.MaintenanceUseCase
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepository;

        public MaintenanceService(IMaintenanceRepository maintenanceRepository)
        {
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<OperationResult<AllMaintenanceViewModel>> GetAllMaintenance(FindMaintenanceCreterias maintenanceCreterias)
        {
            try
            {
                var result = await _maintenanceRepository.GetAllMaintenance(maintenanceCreterias);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<AllMaintenanceViewModel>> GetAllMaintenanceTechnical(FindMaintenanceTechnicalCreterias maintenanceCreterias)
        {
            try
            {
                var result = await _maintenanceRepository.GetAllMaintenanceTechnical(maintenanceCreterias);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<MaintenanceViewModel>> GetMaintenanceDetail(int id)
        {
            try
            {
                var result = await _maintenanceRepository.GetMaintenanceDetail(id);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<string>> CreateMaintenance(CreateMaintenanceModel model)
        {
            try
            {
                var result = await _maintenanceRepository.CreateMaintenance(model);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<string>> UpdateStatusMaintenance(UpdateMaintenanceStatusModel model)
        {
            try
            {
                var result = await _maintenanceRepository.UpdateStatusMaintenance(model);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<string>> CreateItemRequest(CreateItemRequestModel model)
        {
            try
            {
                var result = await _maintenanceRepository.CreateItemRequest(model);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<string>> CreatePurchaseRequest(CreatePurchaseRequestModel model)
        {
            try
            {
                var result = await _maintenanceRepository.CreatePurchaseRequest(model);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<string>> CreatePurchaseRequestService(CreatePurchaseRequestServiceModel model)
        {
            try
            {
                var result = await _maintenanceRepository.CreatePurchaseRequestService(model);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OperationResult<string>> SaveCheckList(SaveCheckListModel model)
        {
            try
            {
                var result = await _maintenanceRepository.SaveCheckList(model);
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
