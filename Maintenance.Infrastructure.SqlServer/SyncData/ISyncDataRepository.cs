using Maintenance.Entities.Maintenance;
using Maintenance.Entities.SyncData;
using Maintenance.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.SyncData
{
    public interface ISyncDataRepository
    {
        public List<UserViewModel> GetAllUser();
        public List<EquipmentViewModel> GetAllEquipment();
        public List<MachineViewModel> GetAllMachine();
        public List<MachineCheckListViewModel> GetAllMachineCheckList();
        public List<MachineSparePartViewModel> GetAllMachineSparePart();
        public List<MaintenanceViewModel> GetAllMaintenance();
        public Task InsertOrUpdateAccount(List<UserViewModel> model);
        public Task InsertOrUpdateEquipment(List<EquipmentViewModel> equipments);
        public Task InsertOrUpdateMaintenance(List<MaintenanceViewModel> maintenances);
        public Task InsertOrUpdateMachine(List<MachineViewModel> machines, List<MachineCheckListViewModel> machineCheckLists, List<MachineSparePartViewModel> machineSpareParts);
    }
}
