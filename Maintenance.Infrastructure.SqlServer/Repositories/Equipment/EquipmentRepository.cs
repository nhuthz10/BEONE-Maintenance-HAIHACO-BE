using AutoMapper;
using Equipment.Infrastructure.SqlServer.Repositories.Equipment;
using Maintenance.Entities.Equipment;
using Maintenance.Entities.Maintenance;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Common;
using Maintenance.Infrastructure.SqlServer.Data;
using Maintenance.Infrastructure.SqlServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equipment.Infrastructure.SqlServer.Repositories.Equipment
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataContextSql _dataContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public EquipmentRepository(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration, DataContextSql dataContextSql)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _dataContext = dataContextSql;
        }

        public async Task<OperationResult<List<EquipmentViewModel>>> GetAllEquipment()
        {
            try
            {
                string query = "B1CS_GET_ALL_EQUIPMENT";

                var dataRows = _dataContext.ExecuteStoredProcedureRawMultiple(query, DataContextSql.SqlDbTarget.Default);


                var headers = dataRows[0]
                .Select(p => new EquipmentViewModel
                {
                    Id = p.GetLong("Id") ?? 0,
                    ItemCode = p.GetString("ItemCode"),
                    ItemName = p.GetString("ItemName"),
                    Uom = p.GetString("Uom"),
                    FactoryCode = p.GetString("FactoryCode"),
                    FactoryName = p.GetString("FactoryName"),
                    Line = p.GetString("Line"),
                    LineName = p.GetString("LineName"),
                    Area = p.GetString("Area"),
                    Section = p.GetString("Section"),
                    SubCSection = p.GetString("SubCSection"),
                    Onhand = p.GetDouble("Onhand"),
                    Manufacturer = p.GetString("Manufacturer"),
                    Origin = p.GetString("Origin"),
                    Model = p.GetString("Model"),
                    MnfYear = p.GetInt("MnfYear"),
                    UsedDate = p.GetDateTime("UsedDate"),
                    MaintCycle = p.GetString("MaintCycle"),
                    MaintCycleType = p.GetString("MaintCycleType"),
                    EquipManager = p.GetString("EquipManager"),
                    Power = p.GetString("Power"),
                    Voltage = p.GetString("Voltage"),
                    LastMaintBy = p.GetString("LastMaintBy"),
                    LastMaintDate = p.GetDateTime("LastMaintDate"),
                    NextMaintDate = p.GetDateTime("NextMaintDate"),
                    ReminderDays = p.GetInt("ReminderDays"),
                    DefaultWhsPR = p.GetString("DefaultWhsPR"),
                    DefaultWhsGI = p.GetString("DefaultWhsGI"),
                    IsNoti = p.GetInt("IsNoti"),
                    IsActive = p.GetString("IsActive"),
                })
                .ToList();

                var checkLists = dataRows[1].Select(p => new
                {
                    EquipmentId = p.GetLong("EquipmentId") ?? 0,

                    ViewModel = new EquipmentCheckListViewModel
                    {
                        JobType = p.GetString("JobType"),
                        CheckItem = p.GetString("CheckItem"),
                        EvaluationStandard = p.GetString("EvaluationStandard"),
                        PassStatus = p.GetString("PassStatus"),
                        Remark = p.GetString("Remark"),
                        Situation = p.GetString("Situation"),
                        Solution = p.GetString("Solution"),
                        IsActive = p.GetString("IsActive"),
                    }
                }).ToList();


                var spareParts = dataRows[2].Select(p => new
                {
                    EquipmentId = p.GetLong("EquipmentId") ?? 0,

                    ViewModel = new EquipmentSparePartViewModel
                    {
                        ItemCode = p.GetString("ItemCode"),
                        ItemName = p.GetString("ItemName"),
                        UomCode = p.GetString("UomCode"),
                        Quantity = p.GetDouble("Quantity"),
                        IsRequired = p.GetString("IsRequired"),
                        IsActive = p.GetString("IsActive"),
                    }
                }).ToList();

                var checklistLookup = checkLists.ToLookup(x => x.EquipmentId);

                var sparePartLookup = spareParts.ToLookup(x => x.EquipmentId);


                foreach (var equipment in headers)
                {
                    equipment.CheckLists = checklistLookup[equipment.Id]
                        .Select(x => x.ViewModel)
                        .ToList();

                    equipment.SpareParts = sparePartLookup[equipment.Id]
                        .Select(x => x.ViewModel)
                        .ToList();
                }

                return OperationResult<List<EquipmentViewModel>>.Success(message: "Get all equipment successfully", data: headers);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OperationResult<List<EquipmentViewModel>>> GetAllSparePart(int id)
        {
            try
            {
                var maintenance = await _context.Maintenances.FirstOrDefaultAsync(m => m.Id == id);

                if (maintenance == null)
                    return OperationResult<List<EquipmentViewModel>>.Fail(ErrorCode.NotFound, "Can not find maintenance");

                string query = "B1CS_GET_ALL_SPAREPART";

                var parameters = new[]
                {
                    new SqlParameter("@WhsCode", SqlDbType.NVarChar)
                    {
                        Value = maintenance.DefaultWhsGI
                    },
                };

                var dataRows = _dataContext.ExecuteStoredProcedureRawMultiple(query, DataContextSql.SqlDbTarget.Default, parameters);

                var headers = dataRows[0]
                .Select(p => new EquipmentViewModel
                {
                    Id = p.GetLong("Id") ?? 0,
                    ItemCode = p.GetString("ItemCode"),
                    ItemName = p.GetString("ItemName"),
                    Uom = p.GetString("Uom"),
                    FactoryCode = p.GetString("FactoryCode"),
                    FactoryName = p.GetString("FactoryName"),
                    Line = p.GetString("Line"),
                    LineName = p.GetString("LineName"),
                    Area = p.GetString("Area"),
                    Section = p.GetString("Section"),
                    SubCSection = p.GetString("SubCSection"),
                    Onhand = p.GetDouble("WhsOnHand"),
                    Manufacturer = p.GetString("Manufacturer"),
                    Origin = p.GetString("Origin"),
                    Model = p.GetString("Model"),
                    MnfYear = p.GetInt("MnfYear"),
                    UsedDate = p.GetDateTime("UsedDate"),
                    MaintCycle = p.GetString("MaintCycle"),
                    MaintCycleType = p.GetString("MaintCycleType"),
                    EquipManager = p.GetString("EquipManager"),
                    Power = p.GetString("Power"),
                    Voltage = p.GetString("Voltage"),
                    LastMaintBy = p.GetString("LastMaintBy"),
                    LastMaintDate = p.GetDateTime("LastMaintDate"),
                    NextMaintDate = p.GetDateTime("NextMaintDate"),
                    ReminderDays = p.GetInt("ReminderDays"),
                    IsActive = p.GetString("IsActive"),
                })
                .ToList();

                var checkLists = dataRows[1].Select(p => new
                {
                    EquipmentId = p.GetLong("EquipmentId") ?? 0,

                    ViewModel = new EquipmentCheckListViewModel
                    {
                        JobType = p.GetString("JobType"),
                        CheckItem = p.GetString("CheckItem"),
                        EvaluationStandard = p.GetString("EvaluationStandard"),
                        PassStatus = p.GetString("PassStatus"),
                        Remark = p.GetString("Remark"),
                        IsActive = p.GetString("IsActive"),
                    }
                }).ToList();


                var spareParts = dataRows[2].Select(p => new
                {
                    EquipmentId = p.GetLong("EquipmentId") ?? 0,

                    ViewModel = new EquipmentSparePartViewModel
                    {
                        ItemCode = p.GetString("ItemCode"),
                        ItemName = p.GetString("ItemName"),
                        UomCode = p.GetString("UomCode"),
                        Quantity = p.GetDouble("Quantity"),
                        IsRequired = p.GetString("IsRequired"),
                        IsActive = p.GetString("IsActive"),
                    }
                }).ToList();

                var checklistLookup = checkLists.ToLookup(x => x.EquipmentId);

                var sparePartLookup = spareParts.ToLookup(x => x.EquipmentId);


                foreach (var equipment in headers)
                {
                    equipment.CheckLists = checklistLookup[equipment.Id]
                        .Select(x => x.ViewModel)
                        .ToList();

                    equipment.SpareParts = sparePartLookup[equipment.Id]
                        .Select(x => x.ViewModel)
                        .ToList();
                }

                return OperationResult<List<EquipmentViewModel>>.Success(message: "Get all spare part successfully", data: headers);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OperationResult<List<LineViewModel>>> GetAllLine()
        {
            try
            {
                string query = "B1CS_GET_ALL_LINE";

                var dataRows = _dataContext.ExecuteStoredProcedureRawMultiple(query, DataContextSql.SqlDbTarget.Default);

                var result = dataRows[0]
                    .Select(p => new LineViewModel
                    {
                        Code = p.GetString("Code"),
                        Name = p.GetString("Name"),
                    })
                    .ToList();

                return OperationResult<List<LineViewModel>>.Success(message: "Get all line successfully", data: result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
