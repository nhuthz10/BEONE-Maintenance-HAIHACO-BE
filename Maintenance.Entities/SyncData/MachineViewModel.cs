using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Entities.SyncData
{
    public class MachineViewModel
    {
        public string Code { get; set; }
        public string? Name { get; set; }
        public int? DocEntry { get; set; }
        public int? ManuYear { get; set; }
        public DateTime? UseDate { get; set; }
        public string? TechPIC { get; set; }
        public string? EquipPIC { get; set; }
        public string? Model { get; set; }
        public string? Brand { get; set; }
        public int? PMCycle { get; set; }
        public string? PMCycleType { get; set; }
        public string? Power { get; set; }
        public string? Voltage { get; set; }
        public int? ReminderDays { get; set; }
        public string? Area { get; set; }
        public string? Section { get; set; }
        public string? SubSection { get; set; }
        public string? Factory { get; set; }
        public string? FactoryName { get; set; }
        public string? Line { get; set; }
        public string? LineName { get; set; }
        public DateTime? LastPMDate { get; set; }
        public string? LastPMPIC { get; set; }
        public DateTime? NextPMDate { get; set; }
        public string? DefaultWhsPR { get; set; }
        public string? DefaultWhsGI { get; set; }
    }
}
