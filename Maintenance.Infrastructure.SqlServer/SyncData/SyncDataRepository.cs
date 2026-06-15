using AutoMapper;
using Maintenance.Entities.Maintenance;
using Maintenance.Entities.SyncData;
using Maintenance.Entities.User;
using Maintenance.Infrastructure.SqlServer.Common;
using Maintenance.Infrastructure.SqlServer.Data;
using Maintenance.Infrastructure.SqlServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.SyncData
{
    public class SyncDataRepository : ISyncDataRepository
    {
        private readonly DataContextSql _context;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly string _dataBaseNameHAIHACO;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public SyncDataRepository(DataContextSql context, ApplicationDbContext dbContext, IMapper mapper, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _dataBaseNameHAIHACO = configuration["Connections:Sap:Company"] ?? "";
        }

        public List<UserViewModel> GetAllUser()
        {
            string query = "B1CS_GET_USER_MAINTENANCE";

            _context.OpenConnection();

            var dataRows = _context.ExecuteStoredProcedureRaw(query, DataContextSql.SqlDbTarget.HaiHaCo);

            var result = dataRows
                .Select(d =>
                {
                    return new UserViewModel
                    {
                        UserCode = Convert.ToString(d["UserCode"]),
                        UserName = Convert.ToString(d["UserName"]),
                        Email = Convert.ToString(d["Email"]),
                        Department = Convert.ToString(d["Department"]),
                        DepartmentDes = Convert.ToString(d["DepartmentDes"]),
                        Position = Convert.ToString(d["Position"]),
                    };
                })
                .ToList();

            _context.CloseConnection();

            return result;
        }

        public List<EquipmentViewModel> GetAllEquipment()
        {
            string query = "B1CS_APP_MAINTENENCE_GETS_ITEM";

            _context.OpenConnection();

            var dataRows = _context.ExecuteStoredProcedureRaw(query, DataContextSql.SqlDbTarget.HaiHaCo);

            var result = dataRows
                .Select(d =>
                {
                    return new EquipmentViewModel
                    {
                        ItemCode = Convert.ToString(d["ItemCode"]),
                        ItemName = Convert.ToString(d["ItemName"]),
                        SubCat = Convert.ToString(d["SubCat"]),
                        UomCode = Convert.ToString(d["UomCode"]),
                        HHCCode = Convert.ToString(d["HHCCode"]),
                        OnHand = Convert.ToDouble(d["OnHand"]),
                        IsActive = Convert.ToString(d["IsActive"]),
                    };
                })
                .ToList();

            _context.CloseConnection();

            return result;
        }

        public List<MachineViewModel> GetAllMachine()
        {
            string query = "B1CS_APP_MAINTENENCE_GETS_MACHINE";

            _context.OpenConnection();

            var dataRows = _context.ExecuteStoredProcedureRaw(query, DataContextSql.SqlDbTarget.HaiHaCo);

            var result = dataRows
                .Select(d =>
                {
                    return new MachineViewModel
                    {
                        Code = Convert.ToString(d["Code"]),
                        Name = d["Name"] == DBNull.Value ? null : Convert.ToString(d["Name"]),
                        DocEntry = d["DocEntry"] == DBNull.Value ? null : Convert.ToInt16(d["DocEntry"]),
                        ManuYear = d["ManuYear"] == DBNull.Value ? null : Convert.ToInt16(d["ManuYear"]),
                        UseDate = d["UseDate"] == DBNull.Value ? null : Convert.ToDateTime(d["UseDate"]),
                        TechPIC = d["TechPIC"] == DBNull.Value ? null : Convert.ToString(d["TechPIC"]),
                        EquipPIC = d["EquipPIC"] == DBNull.Value ? null : Convert.ToString(d["EquipPIC"]),
                        Model = d["Model"] == DBNull.Value ? null : Convert.ToString(d["Model"]),
                        Brand = d["Brand"] == DBNull.Value ? null : Convert.ToString(d["Brand"]),
                        PMCycle = d["PMCycle"] == DBNull.Value ? null : Convert.ToInt16(d["PMCycle"]),
                        PMCycleType = d["PMCycleType"] == DBNull.Value ? null : Convert.ToString(d["PMCycleType"]),
                        Power = d["Power"] == DBNull.Value ? null : Convert.ToString(d["Power"]),
                        Voltage = d["Voltage"] == DBNull.Value ? null : Convert.ToString(d["Voltage"]),
                        ReminderDays = d["ReminderDays"] == DBNull.Value ? null : Convert.ToInt16(d["ReminderDays"]),
                        Area = d["Area"] == DBNull.Value ? null : Convert.ToString(d["Area"]),
                        Section = d["Section"] == DBNull.Value ? null : Convert.ToString(d["Section"]),
                        SubSection = d["SubSection"] == DBNull.Value ? null : Convert.ToString(d["SubSection"]),
                        Factory = d["Factory"] == DBNull.Value ? null : Convert.ToString(d["Factory"]),
                        FactoryName = d["FactoryName"] == DBNull.Value ? null : Convert.ToString(d["FactoryName"]),
                        Line = d["Line"] == DBNull.Value ? null : Convert.ToString(d["Line"]),
                        LastPMDate = d["LastPMDate"] == DBNull.Value ? null : Convert.ToDateTime(d["LastPMDate"]),
                        LastPMPIC = d["LastPMPIC"] == DBNull.Value ? null : Convert.ToString(d["LastPMPIC"]),
                        NextPMDate = d["NextPMDate"] == DBNull.Value ? null : Convert.ToDateTime(d["NextPMDate"]),
                    };
                })
                .ToList();

            _context.CloseConnection();

            return result;
        }

        public List<MaintenanceViewModel> GetAllMaintenance()
        {
            string query = "B1CS_GET_MAINTENENCE_SAP";

            _context.OpenConnection();

            var dataRows = _context.ExecuteStoredProcedureRaw(query, DataContextSql.SqlDbTarget.Default);

            var result = dataRows
                .Select(p =>
                {
                    return new MaintenanceViewModel
                    {
                        Id = p.GetLong("Id") ?? 0,
                        DocNo = p.GetString("DocNo"),
                        MtnType = p.GetInt("MtnType") ?? 0,
                        ItemCode = p.GetString("ItemCode"),
                        DocDate = p.GetDateTime("DocDate") ?? DateTime.MinValue,
                        DueDate = p.GetDateTime("DueDate") ?? DateTime.MinValue,
                        PlannedCompletionDate = p.GetDateTime("PlannedCompletionDate") ?? DateTime.MinValue,
                        RequestUserCode = p.GetString("RequestUserCode"),
                        RequestUserName = p.GetString("RequestUserName"),
                        Department = p.GetString("Department"),
                        DepartmentDes = p.GetString("DepartmentDes"),
                        AssignUser = p.GetString("AssignUser"),
                        AssignUserName = p.GetString("AssignUserName"),
                        AssignUserDepartment = p.GetString("AssignUserDepartment"),
                        AssignUserDepartmentDes = p.GetString("AssignUserDepartmentDes"),
                        FactoryCode = p.GetString("FactoryCode"),
                        FactoryName = p.GetString("FactoryName"),
                        ApproveStatus = p.GetInt("ApproveStatus") ?? 0,
                        RejectedReason = p.GetString("RejectedReason"),
                        Status = p.GetInt("Status") ?? 0,
                        Remark = p.GetString("Remark"),
                        Dscription = p.GetString("Dscription"),
                        CreatedBy = p.GetString("UserNameCreated"),
                        CreatedDate = p.GetDateTime("CreatedDate") ?? DateTime.MinValue,
                        UpdatedBy = p.GetString("UserNameUpdated"),
                        UpdatedDate = p.GetDateTime("UpdatedDate"),
                    };
                })
                .ToList();

            _context.CloseConnection();

            return result;
        }

        public List<MachineCheckListViewModel> GetAllMachineCheckList()
        {
            string query = "B1CS_APP_MAINTENENCE_GETS_MACHINE_CHECKLIST";

            _context.OpenConnection();

            var dataRows = _context.ExecuteStoredProcedureRaw(query, DataContextSql.SqlDbTarget.HaiHaCo);

            var result = dataRows
                .Select(d =>
                {
                    return new MachineCheckListViewModel
                    {
                        Code = Convert.ToString(d["Code"]),
                        Task = d["Task"] == DBNull.Value ? null : Convert.ToString(d["Task"]),
                        TaskGroup = d["TaskGroup"] == DBNull.Value ? null : Convert.ToString(d["TaskGroup"]),
                        Assignee = d["Assignee"] == DBNull.Value ? null : Convert.ToString(d["Assignee"]),
                        Duration = d["Duration"] == DBNull.Value ? null : Convert.ToInt16(d["Duration"]),
                        Required =  d["Required"] == DBNull.Value ? null : Convert.ToString(d["Required"]),
                        EvaluationStandard = d["EvaluationStandard"] == DBNull.Value ? null : Convert.ToString(d["EvaluationStandard"]),
                    };
                })
                .ToList();

            _context.CloseConnection();

            return result;
        }

        public List<MachineSparePartViewModel> GetAllMachineSparePart()
        {
            string query = "B1CS_APP_MAINTENENCE_GETS_MACHINE_SPAREPART";

            _context.OpenConnection();

            var dataRows = _context.ExecuteStoredProcedureRaw(query, DataContextSql.SqlDbTarget.HaiHaCo);

            var result = dataRows
                .Select(d =>
                {
                    return new MachineSparePartViewModel
                    {
                        Code = Convert.ToString(d["Code"]),
                        ItemCode = d["ItemCode"] == DBNull.Value ? null : Convert.ToString(d["ItemCode"]),
                        Dscription = d["Dscription"] == DBNull.Value ? null : Convert.ToString(d["Dscription"]),
                        Uom = d["Uom"] == DBNull.Value ? null : Convert.ToString(d["Uom"]),
                        Quantity = d["Quantity"] == DBNull.Value ? null : Convert.ToDouble(d["Quantity"]),
                        Father = d["Father"] == DBNull.Value ? null : Convert.ToString(d["Father"]),
                        Maintenance = d["Maintenance"] == DBNull.Value ? null : Convert.ToInt16(d["Maintenance"]),
                    };
                })
                .ToList();

            _context.CloseConnection();

            return result;
        }

        public async Task InsertOrUpdateAccount(List<UserViewModel> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == item.UserCode);

                    ApplicationUser account;

                    if (existingUser == null)
                    {
                        account = new ApplicationUser
                        {
                            UserName = item.UserCode,
                            FullName = item.UserName,
                            Email = $"{item.UserCode.ToLower()}@haihaco.vn",
                            Department = item.Department,
                            DepartmentDes = item.DepartmentDes,
                            Position = item.Position,
                        };

                        var result = await _userManager.CreateAsync(account, "123456aB@");

                        if (!result.Succeeded)
                        {
                            var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
                            throw new Exception($"Failed to create user account: {errorMsg}");
                        }

                        //var accountResut = await _userManager.AddToRoleAsync(account, "User");
                        //if (!accountResut.Succeeded)
                        //{
                        //    throw new Exception($"Failed to assign role to user");
                        //}
                    }
                    else
                    {
                        existingUser.FullName = item.UserName;
                        existingUser.Email = $"{item.UserCode.ToLower()}@haihaco.vn";
                        existingUser.Department = item.Department;
                        existingUser.DepartmentDes = item.DepartmentDes;
                        existingUser.Position = item.Position;

                        var result = await _userManager.UpdateAsync(existingUser);

                        if (!result.Succeeded)
                        {
                            var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
                            throw new Exception($"Failed to update user account: {errorMsg}");
                        }

                        //var accountResut = await _userManager.AddToRoleAsync(account, "User");
                        //if (!accountResut.Succeeded)
                        //{
                        //    throw new Exception($"Failed to assign role to user");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task InsertOrUpdateEquipment(List<EquipmentViewModel> equipments)
        {
            try
            {
                string procedureName = "B1CS_SYNC_EQUIPMENTS";

                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("ItemCode", typeof(string));
                table.Columns.Add("ItemName", typeof(string));
                table.Columns.Add("SubCat", typeof(string));
                table.Columns.Add("UomCode", typeof(string));
                table.Columns.Add("HHCCode", typeof(string));
                table.Columns.Add("OnHand", typeof(double));
                table.Columns.Add("IsActive", typeof(string));


                foreach (var equipment in equipments)
                {
                    table.Rows.Add(equipment.ItemCode, equipment.ItemName, equipment.SubCat, equipment.UomCode, equipment.HHCCode, equipment.OnHand, equipment.IsActive);
                }

                var parameters = new[]
                {
                    new SqlParameter("@Equipments", SqlDbType.Structured)
                    {
                        TypeName = "EQUIPMENT_TYPE",
                        Value = table
                    },
                };

                _context.ExecuteNonQueryStoredProcedure(procedureName, DataContextSql.SqlDbTarget.Default, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task InsertOrUpdateMachine(List<MachineViewModel> machines, List<MachineCheckListViewModel> machineCheckLists, List<MachineSparePartViewModel> machineSpareParts)
        {
            try
            {
                string procedureName = "B1CS_SYNC_MACHINES";

                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("Code", typeof(string));
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("DocEntry", typeof(int));
                table.Columns.Add("ManuYear", typeof(int));
                table.Columns.Add("UseDate", typeof(DateTime));
                table.Columns.Add("TechPIC", typeof(string));
                table.Columns.Add("EquipPIC", typeof(string));
                table.Columns.Add("Model", typeof(string));
                table.Columns.Add("Brand", typeof(string));
                table.Columns.Add("PMCycle", typeof(int));
                table.Columns.Add("PMCycleType", typeof(string));
                table.Columns.Add("Power", typeof(string));
                table.Columns.Add("Voltage", typeof(string));
                table.Columns.Add("ReminderDays", typeof(int));
                table.Columns.Add("Area", typeof(string));
                table.Columns.Add("Section", typeof(string));
                table.Columns.Add("SubSection", typeof(string));
                table.Columns.Add("Factory", typeof(string));
                table.Columns.Add("FactoryName", typeof(string));
                table.Columns.Add("Line", typeof(string));
                table.Columns.Add("LastPMDate", typeof(DateTime));
                table.Columns.Add("LastPMPIC", typeof(string));
                table.Columns.Add("NextPMDate", typeof(DateTime));

                System.Data.DataTable tableCheckList = new System.Data.DataTable();
                tableCheckList.Columns.Add("Code", typeof(string));
                tableCheckList.Columns.Add("LineId", typeof(int));
                tableCheckList.Columns.Add("Task", typeof(string));
                tableCheckList.Columns.Add("TaskGroup", typeof(string));
                tableCheckList.Columns.Add("Assignee", typeof(string));
                tableCheckList.Columns.Add("Duration", typeof(int));
                tableCheckList.Columns.Add("Maintenance", typeof(int));
                tableCheckList.Columns.Add("Required", typeof(string));
                tableCheckList.Columns.Add("EvaluationStandard", typeof(string));

                System.Data.DataTable tableSparePart = new System.Data.DataTable();
                tableSparePart.Columns.Add("Code", typeof(string));
                tableSparePart.Columns.Add("LineId", typeof(int));
                tableSparePart.Columns.Add("ItemCode", typeof(string));
                tableSparePart.Columns.Add("Dscription", typeof(string));
                tableSparePart.Columns.Add("Uom", typeof(string));
                tableSparePart.Columns.Add("Quantity", typeof(double));
                tableSparePart.Columns.Add("Maintenance", typeof(string));
                tableSparePart.Columns.Add("Father", typeof(string));


                foreach (var machine in machines)
                {
                    table.Rows.Add(
                        machine.Code,
                        machine.Name,
                        machine.DocEntry,
                        machine.ManuYear,
                        machine.UseDate,
                        machine.TechPIC,
                        machine.EquipPIC,
                        machine.Model,
                        machine.Brand,
                        machine.PMCycle,
                        machine.PMCycleType,
                        machine.Power,
                        machine.Voltage,
                        machine.ReminderDays,
                        machine.Area,
                        machine.Section,
                        machine.SubSection,
                        machine.Factory,
                        machine.FactoryName,
                        machine.Line,
                        machine.LastPMDate,
                        machine.LastPMPIC,
                        machine.NextPMDate
                    );
                }

                foreach (var item in machineCheckLists)
                {
                    tableCheckList.Rows.Add(
                        item.Code,
                        item.LineId,
                        item.Task,
                        item.TaskGroup,
                        item.Assignee,
                        item.Duration,
                        item.Maintenance,
                        item.Required,
                        item.EvaluationStandard
                    );
                }

                foreach (var item in machineSpareParts)
                {
                    tableSparePart.Rows.Add(
                        item.Code,
                        item.LineId,
                        item.ItemCode,
                        item.Dscription,
                        item.Uom,
                        item.Quantity,
                        item.Maintenance,
                        item.Father
                    );
                }

                var parameters = new[]
                {
                    new SqlParameter("@Machines", SqlDbType.Structured)
                    {
                        TypeName = "MACHINE_TYPE",
                        Value = table
                    },
                    new SqlParameter("@MachineSpareParts", SqlDbType.Structured)
                    {
                        TypeName = "MACHINE_SPAREPART_TYPE",
                        Value = tableSparePart
                    },
                    new SqlParameter("@MachineChecklists", SqlDbType.Structured)
                    {
                        TypeName = "MACHINE_CHECKLIST_TYPE",
                        Value = tableCheckList
                    },
                };

                _context.ExecuteNonQueryStoredProcedure(procedureName, DataContextSql.SqlDbTarget.Default, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task InsertOrUpdateMaintenance(List<MaintenanceViewModel> maintenances)
        {
            try
            {
                string procedureName = "B1CS_APP_MAINTENANCE_INSERT_OR_UPDATE";

                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("DocNo", typeof(string));
                table.Columns.Add("MtnType", typeof(int));
                table.Columns.Add("ItemCode", typeof(string));
                table.Columns.Add("FactoryCode", typeof(string));
                table.Columns.Add("FactoryName", typeof(string));
                table.Columns.Add("DocDate", typeof(DateTime));
                table.Columns.Add("DueDate", typeof(DateTime));
                table.Columns.Add("PlannedCompletionDate", typeof(DateTime));
                table.Columns.Add("RequestUserCode", typeof(string));
                table.Columns.Add("RequestUserName", typeof(string));
                table.Columns.Add("Department", typeof(string));
                table.Columns.Add("DepartmentDes", typeof(string));
                table.Columns.Add("AssignUser", typeof(string));
                table.Columns.Add("AssignUserName", typeof(string));
                table.Columns.Add("AssignUserDepartment", typeof(string));
                table.Columns.Add("AssignUserDepartmentDes", typeof(string));
                table.Columns.Add("ApproveStatus", typeof(int));
                table.Columns.Add("RejectedReason", typeof(string));
                table.Columns.Add("Status", typeof(int));
                table.Columns.Add("Remark", typeof(string));
                table.Columns.Add("Dscription", typeof(string));
                table.Columns.Add("CreatedBy", typeof(string));
                table.Columns.Add("CreatedDate", typeof(DateTime));
                table.Columns.Add("UpdatedBy", typeof(string));
                table.Columns.Add("UpdatedDate", typeof(DateTime));

                foreach (var maintenance in maintenances)
                {
                    table.Rows.Add(
                        maintenance.Id,
                        maintenance.DocNo,
                        maintenance.MtnType,
                        maintenance.ItemCode,
                        maintenance.FactoryCode,
                        maintenance.FactoryName,
                        maintenance.DocDate,
                        maintenance.DueDate,
                        maintenance.PlannedCompletionDate,
                        maintenance.RequestUserCode,
                        maintenance.RequestUserName,
                        maintenance.Department,
                        maintenance.DepartmentDes,
                        maintenance.AssignUser,
                        maintenance.AssignUserName,
                        maintenance.AssignUserDepartment,
                        maintenance.AssignUserDepartmentDes,
                        maintenance.ApproveStatus,
                        maintenance.RejectedReason,
                        maintenance.Status,
                        maintenance.Remark,
                        maintenance.Dscription,
                        maintenance.CreatedBy,
                        maintenance.CreatedDate,
                        maintenance.UpdatedBy,
                        maintenance.UpdatedDate
                    );
                }


                var parameters = new[]
                {
                    new SqlParameter("@Maintenances", SqlDbType.Structured)
                    {
                        TypeName = "B1CS_TYPE_MAINTENANCE",
                        Value = table
                    },
                };

                _context.ExecuteNonQueryStoredProcedure(procedureName, DataContextSql.SqlDbTarget.HaiHaCo, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
