using Hangfire;
using Maintenance.Infrastructure.SqlServer.Repositories.Maintenance;
using Maintenance.Infrastructure.SqlServer.SyncData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.SyncDataService
{
    public class SyncDataService
    {
        private readonly ISyncDataRepository _syncDataRepository;
        private readonly IMaintenanceRepository _maintenanceRepository;

        public SyncDataService(ISyncDataRepository syncDataRepository, IMaintenanceRepository maintenanceRepository)
        {
            _syncDataRepository = syncDataRepository;
            _maintenanceRepository = maintenanceRepository;
        }

        public void CreateSyncDataUserJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("sync-user",
                methodCall: () => SyncDataUser(),
                 "*/30 * * * *", options: options);
        }

        public void CreateSyncEquipmentJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("create-equipment",
                methodCall: () => CreateEquipment(),
                 "*/1 * * * *", options: options);
        }

        public void CreateSyncMachineJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("create-machine",
                methodCall: () => CreateMachine(),
                 "*/3 * * * *", options: options);
        }

        public void CreateMaintenancePeriodicJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("create-maintenance-periodic",
                methodCall: () => CreateMaintenancePeriodic(),
                 "*/30 * * * *", options: options);
        }

        public void CreateMaintenanceJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("create-maintenance",
                methodCall: () => CreateMaintenance(),
                 "*/5 * * * *", options: options);
        }

        public void CreateUpdateMaintenanceCompleteJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("udpate-maintenance-complete",
                methodCall: () => CreateUpdateMaintenanceComplete(),
                 "*/2 * * * *", options: options);
        }

        public void CreateMaintenanceContinueJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("update-maintenance-continue",
                methodCall: () => CreateMaintenanceContinue(),
                 "*/5 * * * *", options: options);
        }

        public async Task SyncDataUser()
        {
            try
            {
                var users = _syncDataRepository.GetAllUser();
                await _syncDataRepository.InsertOrUpdateAccount(users);
            }
            catch
            {
                throw;
            }
        }
        public async Task CreateMaintenancePeriodic()
        {
            try
            {
                await _maintenanceRepository.CreateMaintenancePeriodic();
            }
            catch
            {
                throw;
            }
        }

        public async Task CreateUpdateMaintenanceComplete()
        {
            try
            {
                await _maintenanceRepository.UpdateCompleteStatusMaintenance();
            }
            catch
            {
                throw;
            }
        }

        public async Task CreateMaintenanceContinue()
        {
            try
            {
                await _maintenanceRepository.UpdateMaintenanceContinue();
            }
            catch
            {
                throw;
            }
        }

        public async Task CreateEquipment()
        {
            try
            {
                var equipment = _syncDataRepository.GetAllEquipment();
                await _syncDataRepository.InsertOrUpdateEquipment(equipment);
            }
            catch
            {
                throw;
            }
        }

        public async Task CreateMachine()
        {
            try
            {
                var machine = _syncDataRepository.GetAllMachine();
                var machineCheckList = _syncDataRepository.GetAllMachineCheckList();
                var machineSparePart = _syncDataRepository.GetAllMachineSparePart();
                await _syncDataRepository.InsertOrUpdateMachine(machine, machineCheckList, machineSparePart);
            }
            catch
            {
                throw;
            }
        }

        public async Task CreateMaintenance()
        {
            try
            {
                var maintenance = _syncDataRepository.GetAllMaintenance();
                await _syncDataRepository.InsertOrUpdateMaintenance(maintenance);
            }
            catch
            {
                throw;
            }
        }

    }
}
