using AutoMapper;
using Equipment.Infrastructure.SqlServer.Repositories.Equipment;
using Maintenance.Entities.Maintenance;
using Maintenance.Entities.Responses;
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
using static Maintenance.Infrastructure.SqlServer.Data.DataContextSql;

namespace Maintenance.Infrastructure.SqlServer.Repositories.Maintenance
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly DataContextSql _dataContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly Company _oCompany;

        public MaintenanceRepository(ApplicationDbContext context, IEquipmentRepository equipmentRepository, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration, DataContextSql dataContextSql, Company oCompany)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _dataContext = dataContextSql;
            _oCompany = oCompany;
            _equipmentRepository = equipmentRepository;
        }

        public async Task<OperationResult<AllMaintenanceViewModel>> GetAllMaintenance(FindMaintenanceCreterias maintenanceCreterias)
        {
            try
            {
                string query = "B1CS_GET_ALL_MAINTENANCE";

                var parameters = new[]
                {
                    new SqlParameter("@AccountId", SqlDbType.NVarChar)
                    {
                        Value = maintenanceCreterias.AccountId,
                    },
                    new SqlParameter("@Status", SqlDbType.Int)
                    {
                        Value = maintenanceCreterias.Status
                    },
                };

                var dataRows = _dataContext.ExecuteStoredProcedureRawMultiple(query, DataContextSql.SqlDbTarget.Default, parameters);


                var headers = dataRows[0].Select(p => new MaintenanceViewModel
                {
                    Id = p.GetLong("Id") ?? 0,
                    DocNo = p.GetString("DocNo"),
                    MtnType = p.GetInt("MtnType") ?? 0,
                    ItemCode = p.GetString("ItemCode"),
                    ItemName = p.GetString("ItemName"),
                    FactoryCode = p.GetString("FactoryCode"),
                    FactoryName = p.GetString("FactoryName"),
                    DocDate = p.GetDateTime("DocDate") ?? DateTime.MinValue,
                    DueDate = p.GetDateTime("DueDate") ?? DateTime.MinValue,
                    PlannedCompletionDate = p.GetDateTime("PlannedCompletionDate") ?? DateTime.MinValue,
                    RequestUserCode = p.GetString("RequestUserCode"),
                    RequestUserName = p.GetString("RequestUserName"),
                    AssignUser = p.GetString("AssignUser"),
                    AssignUserName = p.GetString("AssignUserName"),
                    AssignUserDepartment = p.GetString("AssignUserDepartment"),
                    AssignUserDepartmentDes = p.GetString("AssignUserDepartmentDes"),
                    ApproveStatus = p.GetInt("ApproveStatus") ?? 0,
                    RejectedReason = p.GetString("RejectedReason"),
                    Status = p.GetInt("Status") ?? 0,
                    Remark = p.GetString("Remark"),
                    Dscription = p.GetString("Dscription"),
                    CreatedBy = p.GetString("CreatedBy"),
                    CreatedDate = p.GetDateTime("CreatedDate") ?? DateTime.MinValue
                })
                .OrderByDescending(m => m.Id)
                .ToList();


                var documents = dataRows[1].Select(p => new
                {
                    Id = p.GetLong("Id") ?? 0,
                    HeaderId = p.GetLong("HeaderId") ?? 0,

                    ViewModel = new MaintenanceDocViewModel
                    {
                        DocNo = p.GetLong("DocNo"),
                        DocDate = p.GetDateTime("DocDate"),
                        CardCode = p.GetString("CardCode"),
                        DocType = p.GetString("DocType"),
                        ObjectType = p.GetInt("ObjectType"),
                        Machine = p.GetString("Machine"),
                    }
                }).ToList();

                var details = dataRows[2].Select(p => new
                {
                    DocId = p.GetLong("DocId") ?? 0,

                    ViewModel = new MaintenanceDocDetailViewModel
                    {
                        ItemCode = p.GetString("ItemCode"),
                        Description = p.GetString("Description"),
                        Quantity = p.GetDouble("Quantity"),
                        UomCode = p.GetString("UomCode")
                    }
                }).ToList();


                var attachments = dataRows[3].Select(p => new
                {
                    HeaderId = p.GetLong("HeaderId") ?? 0,

                    ViewModel = new MaintenenceAttachmentViewModel
                    {
                        FilePath = p.GetString("FilePath"),
                        FileName = p.GetString("FileName"),
                        FileType = p.GetString("FileType"),
                        Order = p.GetInt("Order")
                    }
                }).ToList();

                var checkLists = dataRows[4].Select(p => new
                {
                    HeaderId = p.GetLong("HeaderId") ?? 0,

                    ViewModel = new MaintenenceCheckListViewModel
                    {
                        Id = p.GetInt("Id") ?? 0,
                        JobType = p.GetString("JobType"),
                        CheckItem = p.GetString("CheckItem"),
                        EvaluationStandard = p.GetString("EvaluationStandard"),
                        DataType = p.GetString("DataType"),
                        CheckResult = p.GetString("CheckResult"),
                        EquipmentStatus = p.GetString("EquipmentStatus"),
                        Remark = p.GetString("Remark")
                    }
                }).ToList();


                var spareParts = dataRows[5].Select(p => new
                {
                    HeaderId = p.GetLong("HeaderId") ?? 0,

                    ViewModel = new MaintenenceSparePartViewModel
                    {
                        ItemCode = p.GetString("ItemCode"),
                        ItemName = p.GetString("ItemName"),
                        UomCode = p.GetString("UomCode"),
                        Quantity = p.GetDouble("Quantity")
                    }
                }).ToList();

                var status = dataRows[6].Select(p => new
                {
                    TotalCount = p.GetInt("TotalCount"),
                    InProgressCount = p.GetInt("InProgressCount"),
                    CompletedCount = p.GetInt("CompletedCount"),
                }).FirstOrDefault();

                var documentLookup = documents.ToLookup(x => x.HeaderId);

                var detailLookup = details.ToLookup(x => x.DocId);

                var attachmentLookup = attachments.ToLookup(x => x.HeaderId);

                var checklistLookup = checkLists.ToLookup(x => x.HeaderId);

                var sparePartLookup = spareParts.ToLookup(x => x.HeaderId);


                foreach (var header in headers)
                {
                    header.MaintenanceDocs = documentLookup[header.Id]
                        .Select(doc =>
                        {
                            doc.ViewModel.Details = detailLookup[doc.Id]
                                .Select(x => x.ViewModel)
                                .ToList();

                            return doc.ViewModel;
                        })
                        .ToList();

                    header.MaintenenceAttachments = attachmentLookup[header.Id]
                        .Select(x => x.ViewModel)
                        .ToList();

                    header.MaintenenceCheckLists = checklistLookup[header.Id]
                        .Select(x => x.ViewModel)
                        .ToList();

                    header.MaintenenceSpareParts = sparePartLookup[header.Id]
                        .Select(x => x.ViewModel)
                        .ToList();
                }

                var result = new AllMaintenanceViewModel
                {
                    Maintenances = headers,
                    TotalCount = status?.TotalCount ?? 0,
                    InProgressCount = status?.InProgressCount ?? 0,
                    CompletedCount = status?.CompletedCount ?? 0,
                };

                return OperationResult<AllMaintenanceViewModel>.Success(message: "Get all maintenance successfully", data: result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OperationResult<AllMaintenanceViewModel>> GetAllMaintenanceTechnical(FindMaintenanceTechnicalCreterias maintenanceCreterias)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(maintenanceCreterias.AccountId);

                if(user == null)
                    return OperationResult<AllMaintenanceViewModel>.Fail(ErrorCode.NotFound, "Can not find user");

                string query = "B1CS_GET_ALL_MAINTENANCE_TECHNICAL";

                var parameters = new[]
                {
                    new SqlParameter("@UserCode", SqlDbType.NVarChar)
                    {
                        Value = user.UserName,
                    },
                    new SqlParameter("@Factory", SqlDbType.NVarChar)
                    {
                        Value = maintenanceCreterias.Factory
                    },
                    new SqlParameter("@MtnType", SqlDbType.Int)
                    {
                        Value = maintenanceCreterias.MtnType
                    },
                };

                var dataRows = _dataContext.ExecuteStoredProcedureRawMultiple(query, DataContextSql.SqlDbTarget.Default, parameters);


                var headers = dataRows[0].Select(p => new MaintenanceViewModel
                {
                    Id = p.GetLong("Id") ?? 0,
                    DocNo = p.GetString("DocNo"),
                    MtnType = p.GetInt("MtnType") ?? 0,
                    ItemCode = p.GetString("ItemCode"),
                    ItemName = p.GetString("ItemName"),
                    FactoryCode = p.GetString("FactoryCode"),
                    FactoryName = p.GetString("FactoryName"),
                    DocDate = p.GetDateTime("DocDate") ?? DateTime.MinValue,
                    DueDate = p.GetDateTime("DueDate"),
                    LastMaintDate = p.GetDateTime("LastMaintDate"),
                    MaintCycle = p.GetString("MaintCycle"),
                    MaintCycleType = p.GetString("MaintCycleType"),
                    PlannedCompletionDate = p.GetDateTime("PlannedCompletionDate"),
                    RequestUserCode = p.GetString("RequestUserCode"),
                    RequestUserName = p.GetString("RequestUserName"),
                    AssignUser = p.GetString("AssignUser"),
                    AssignUserName = p.GetString("AssignUserName"),
                    AssignUserDepartment = p.GetString("AssignUserDepartment"),
                    AssignUserDepartmentDes = p.GetString("AssignUserDepartmentDes"),
                    ApproveStatus = p.GetInt("ApproveStatus") ?? 0,
                    RejectedReason = p.GetString("RejectedReason"),
                    Status = p.GetInt("Status") ?? 0,
                    Remark = p.GetString("Remark"),
                    Dscription = p.GetString("Dscription"),
                    CreatedBy = p.GetString("CreatedBy"),
                    CreatedDate = p.GetDateTime("CreatedDate") ?? DateTime.MinValue
                })
                .OrderByDescending(m => m.Id)
                .ToList();


                var documents = dataRows[1].Select(p => new
                {
                    Id = p.GetLong("Id") ?? 0,
                    HeaderId = p.GetLong("HeaderId") ?? 0,

                    ViewModel = new MaintenanceDocViewModel
                    {
                        DocNo = p.GetLong("DocNo"),
                        DocDate = p.GetDateTime("DocDate"),
                        CardCode = p.GetString("CardCode"),
                        DocType = p.GetString("DocType"),
                        ObjectType = p.GetInt("ObjectType"),
                        Machine = p.GetString("Machine"),
                    }
                }).ToList();

                var details = dataRows[2].Select(p => new
                {
                    DocId = p.GetLong("DocId") ?? 0,

                    ViewModel = new MaintenanceDocDetailViewModel
                    {
                        ItemCode = p.GetString("ItemCode"),
                        Description = p.GetString("Description"),
                        Quantity = p.GetDouble("Quantity"),
                        UomCode = p.GetString("UomCode")
                    }
                }).ToList();


                var attachments = dataRows[3].Select(p => new
                {
                    HeaderId = p.GetLong("HeaderId") ?? 0,

                    ViewModel = new MaintenenceAttachmentViewModel
                    {
                        FilePath = p.GetString("FilePath"),
                        FileName = p.GetString("FileName"),
                        FileType = p.GetString("FileType"),
                        Order = p.GetInt("Order")
                    }
                }).ToList();

                var checkLists = dataRows[4].Select(p => new
                {
                    HeaderId = p.GetLong("HeaderId") ?? 0,

                    ViewModel = new MaintenenceCheckListViewModel
                    {
                        Id = p.GetInt("Id") ?? 0,
                        JobType = p.GetString("JobType"),
                        CheckItem = p.GetString("CheckItem"),
                        EvaluationStandard = p.GetString("EvaluationStandard"),
                        DataType = p.GetString("DataType"),
                        CheckResult = p.GetString("CheckResult"),
                        EquipmentStatus = p.GetString("EquipmentStatus"),
                        Remark = p.GetString("Remark")
                    }
                }).ToList();


                var spareParts = dataRows[5].Select(p => new
                {
                    HeaderId = p.GetLong("HeaderId") ?? 0,

                    ViewModel = new MaintenenceSparePartViewModel
                    {
                        ItemCode = p.GetString("ItemCode"),
                        ItemName = p.GetString("ItemName"),
                        UomCode = p.GetString("UomCode"),
                        Quantity = p.GetDouble("Quantity")
                    }
                }).ToList();

                var status = dataRows[6].Select(p => new
                {
                    TotalCount = p.GetInt("TotalCount"),
                    InProgressCount = p.GetInt("InProgressCount"),
                    CompletedCount = p.GetInt("CompletedCount"),
                    WaitExecutionCount = p.GetInt("WaitExecutionCount"),
                }).FirstOrDefault();

                var documentLookup = documents.ToLookup(x => x.HeaderId);

                var detailLookup = details.ToLookup(x => x.DocId);

                var attachmentLookup = attachments.ToLookup(x => x.HeaderId);

                var checklistLookup = checkLists.ToLookup(x => x.HeaderId);

                var sparePartLookup = spareParts.ToLookup(x => x.HeaderId);


                foreach (var header in headers)
                {
                    header.MaintenanceDocs = documentLookup[header.Id]
                        .Select(doc =>
                        {
                            doc.ViewModel.Details = detailLookup[doc.Id]
                                .Select(x => x.ViewModel)
                                .ToList();

                            return doc.ViewModel;
                        })
                        .ToList();

                    header.MaintenenceAttachments = attachmentLookup[header.Id]
                        .Select(x => x.ViewModel)
                        .ToList();

                    header.MaintenenceCheckLists = checklistLookup[header.Id]
                        .Select(x => x.ViewModel)
                        .ToList();

                    header.MaintenenceSpareParts = sparePartLookup[header.Id]
                        .Select(x => x.ViewModel)
                        .ToList();
                }

                var result = new AllMaintenanceViewModel
                {
                    Maintenances = headers,
                    TotalCount = status?.TotalCount ?? 0,
                    WaitExecutionCount = status?.WaitExecutionCount ?? 0,
                    InProgressCount = status?.InProgressCount ?? 0,
                    CompletedCount = status?.CompletedCount ?? 0,
                };

                return OperationResult<AllMaintenanceViewModel>.Success(message: "Get all maintenance  successfully", data: result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OperationResult<MaintenanceViewModel>> GetMaintenanceDetail(int id)
        {
            try
            {
                string query = "B1CS_GET_MAINTENANCE_DETAIL";

                var parameters = new[]
                {
                    new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Value = id
                    },
                };

                var dataRows = _dataContext.ExecuteStoredProcedureRawMultiple(query, DataContextSql.SqlDbTarget.Default, parameters);

                var header = dataRows[0]
                    .Select(p => new MaintenanceViewModel
                    {
                        Id = p.GetLong("Id") ?? 0,
                        DocNo = p.GetString("DocNo"),
                        MtnType = p.GetInt("MtnType") ?? 0,
                        ItemCode = p.GetString("ItemCode"),
                        ItemName = p.GetString("ItemName"),
                        FactoryCode = p.GetString("FactoryCode"),
                        FactoryName = p.GetString("FactoryName"),
                        DocDate = p.GetDateTime("DocDate") ?? DateTime.MinValue,
                        DueDate = p.GetDateTime("DueDate") ?? DateTime.MinValue,
                        PlannedCompletionDate = p.GetDateTime("PlannedCompletionDate") ?? DateTime.MinValue,
                        RequestUserCode = p.GetString("RequestUserCode"),
                        RequestUserName = p.GetString("RequestUserName"),
                        AssignUser = p.GetString("AssignUser"),
                        AssignUserName = p.GetString("AssignUserName"),
                        AssignUserDepartment = p.GetString("AssignUserDepartment"),
                        AssignUserDepartmentDes = p.GetString("AssignUserDepartmentDes"),
                        ApproveStatus = p.GetInt("ApproveStatus") ?? 0,
                        LastMaintDate = p.GetDateTime("LastMaintDate"),
                        MaintCycle = p.GetString("MaintCycle"),
                        MaintCycleType = p.GetString("MaintCycleType"),
                        RejectedReason = p.GetString("RejectedReason"),
                        Status = p.GetInt("Status") ?? 0,
                        Remark = p.GetString("Remark"),
                        Dscription = p.GetString("Dscription"),
                        CreatedBy = p.GetString("CreatedBy"),
                        CreatedDate = p.GetDateTime("CreatedDate") ?? DateTime.MinValue
                    })
                    .FirstOrDefault();

                if (header == null)
                    return OperationResult<MaintenanceViewModel>.Fail(ErrorCode.NotFound, "Can not find maintenance");

                var documents = dataRows[1].Select(p => new
                {
                    Id = p.GetLong("Id") ?? 0,

                    ViewModel = new MaintenanceDocViewModel
                    {
                        DocNo = p.GetLong("DocNo"),
                        DocDate = p.GetDateTime("DocDate"),
                        CardCode = p.GetString("CardCode"),
                        DocType = p.GetString("DocType"),
                        ObjectType = p.GetInt("ObjectType"),
                        Machine = p.GetString("Machine")
                    }
                }).ToList();

                var details = dataRows[2].Select(p => new
                {
                    DocId = p.GetLong("DocId") ?? 0,

                    ViewModel = new MaintenanceDocDetailViewModel
                    {
                        ItemCode = p.GetString("ItemCode"),
                        Description = p.GetString("Description"),
                        Quantity = p.GetDouble("Quantity"),
                        UomCode = p.GetString("UomCode")
                    }
                }).ToList();

                var attachments = dataRows[3]
                .Select(p => new MaintenenceAttachmentViewModel
                {
                    FilePath = p.GetString("FilePath"),
                    FileName = p.GetString("FileName"),
                    FileType = p.GetString("FileType"),
                    Order = p.GetInt("Order")
                })
                .ToList();

                var checkLists = dataRows[4]
                .Select(p => new MaintenenceCheckListViewModel
                {
                    Id = p.GetInt("Id") ?? 0,
                    JobType = p.GetString("JobType"),
                    CheckItem = p.GetString("CheckItem"),
                    EvaluationStandard = p.GetString("EvaluationStandard"),
                    DataType = p.GetString("DataType"),
                    CheckResult = p.GetString("CheckResult"),
                    EquipmentStatus = p.GetString("EquipmentStatus"),
                    Remark = p.GetString("Remark")
                })
                .ToList();

                var spareParts = dataRows[5]
                .Select(p => new MaintenenceSparePartViewModel
                {
                    ItemCode = p.GetString("ItemCode"),
                    ItemName = p.GetString("ItemName"),
                    UomCode = p.GetString("UomCode"),
                    Quantity = p.GetDouble("Quantity")
                })
                .ToList();

                var detailLookup = details.ToLookup(x => x.DocId);

                header.MaintenanceDocs = documents
                    .Select(doc =>
                    {
                        doc.ViewModel.Details = detailLookup[doc.Id]
                            .Select(x => x.ViewModel)
                            .ToList();

                        return doc.ViewModel;
                    })
                    .ToList();

                header.MaintenenceAttachments = attachments;
                header.MaintenenceCheckLists = checkLists;
                header.MaintenenceSpareParts = spareParts;


                return OperationResult<MaintenanceViewModel>.Success(message: "Get maintenance successfully", data: header);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OperationResult<string>> CreateMaintenance(CreateMaintenanceModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(model.CreatedBy);

                if (user == null)
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find user");


                var item = await _context.Equipments.FirstOrDefaultAsync(i => i.ItemCode == model.ItemCode);

                if (item == null)
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find equipment");

                var maintenanceLast = await _context.Maintenances.OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                var nextNumber = (maintenanceLast?.Id ?? 0) + 1;

                var maintenance = new Maintenances
                {
                    DocNo = $"W-O{nextNumber:D5}",
                    MtnType = model.MtnType,
                    ItemCode = model.ItemCode,
                    FactoryCode = item.FactoryCode,
                    FactoryName = item.FactoryName,
                    DocDate = model.DocDate,
                    PlannedCompletionDate = model.PlannedCompletionDate,
                    RequestUserCode = user.UserName,
                    RequestUserName = user.FullName,
                    Department = user.Department,
                    DepartmentDes = user.DepartmentDes,

                    Dscription = model.Dscription,

                    ApproveStatus = model.MtnType == 0 ? 0 : 1,
                    Status = model.MtnType == 0 ? 2 : 0,

                    CreatedBy = model.CreatedBy,
                    UpdatedBy = model.CreatedBy,

                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Maintenances.Add(maintenance);

                await _context.SaveChangesAsync();

                if (model.Attachments?.Any() == true)
                {
                    var uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "Upload",
                        "Maintenance",
                        maintenance.Id.ToString());

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    var attachmentEntities = new List<MaintenenceAttachments>();

                    foreach (var attachment in model.Attachments)
                    {
                        if (attachment.File == null ||
                            attachment.File.Length == 0)
                        {
                            continue;
                        }

                        var extension = Path.GetExtension(
                            attachment.File.FileName);

                        var fileName =
                            $"{Guid.NewGuid():N}{extension}";

                        var fullPath =
                            Path.Combine(uploadFolder, fileName);

                        await using (var stream = new FileStream(
                            fullPath,
                            FileMode.Create))
                        {
                            await attachment.File.CopyToAsync(stream);
                        }

                        var relativePath =
                            $"/Upload/Maintenance/{maintenance.Id}/{fileName}";

                        var fileType =
                            attachment.File.ContentType.StartsWith("video/")
                                ? "video"
                                : "image";

                        attachmentEntities.Add(
                            new MaintenenceAttachments
                            {
                                HeaderId = maintenance.Id,
                                Type = 0,
                                FilePath = relativePath,
                                FileName = attachment.File.FileName,
                                FileType = fileType,
                                Order = attachment.Order
                            });
                    }

                    if (attachmentEntities.Any())
                    {
                        _context.MaintenenceAttachments.AddRange(
                            attachmentEntities);

                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                return OperationResult<string>.Success(message: "Create maintenance successfully", data: maintenance.DocNo);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CreateMaintenancePeriodic()
        {
            var equipmentRaw = await _equipmentRepository.GetAllEquipment();

            var equipments = equipmentRaw.Data;

            var today = DateTime.Today;

            var maintenanceEquipments = equipments
                .Where(x =>
                    x.NextMaintDate.HasValue &&
                    x.ReminderDays.HasValue &&
                    (x.IsNoti == null || x.IsNoti == 0) &&
                    x.NextMaintDate.Value.Date.AddDays(-x.ReminderDays.Value) <= today)
                .ToList();

            if (!maintenanceEquipments.Any())
                return;

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var lastId = await _context.Maintenances
                    .MaxAsync(x => (long?)x.Id) ?? 0;

                foreach (var equipment in maintenanceEquipments)
                {
                    lastId++;

                    if (equipment.EquipManager == null) continue;
                    var user = await _userManager.FindByNameAsync(equipment.EquipManager);
                    if (user == null) continue;

                    var maintenance = new Maintenances
                    {
                        DocNo = $"W-O{lastId:D5}",
                        MtnType = 1,

                        ItemCode = equipment.ItemCode,
                        FactoryCode = equipment.FactoryCode,
                        FactoryName = equipment.FactoryName,

                        DocDate = DateTime.Now,
                        DueDate = equipment.NextMaintDate ?? DateTime.Now,

                        RequestUserCode = "System",
                        RequestUserName = "System",

                        AssignUser = user?.UserName,
                        AssignUserName = user?.FullName,
                        AssignUserDepartment = user?.Department,
                        AssignUserDepartmentDes = user?.DepartmentDes,

                        ApproveStatus = 1,
                        Status = 0,

                        Dscription = equipment.ItemName,

                        CreatedBy = "System",
                        UpdatedBy = "System",

                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };

                    await _context.Maintenances.AddAsync(maintenance);
                    await _context.SaveChangesAsync();

                    if (equipment.CheckLists?.Any() == true)
                    {
                        var checkLists = equipment.CheckLists
                            .Select(x => new MaintenenceCheckLists
                            {
                                HeaderId = maintenance.Id,
                                JobType = x.JobType,
                                CheckItem = x.CheckItem,
                                EvaluationStandard = x.EvaluationStandard,
                                Remark = x.Remark,
                            })
                            .ToList();

                        await _context.MaintenenceCheckLists
                            .AddRangeAsync(checkLists);
                    }

                    if (equipment.SpareParts?.Any() == true)
                    {
                        var spareParts = equipment.SpareParts
                            .Select(x => new MaintenenceSpareParts
                            {
                                HeaderId = maintenance.Id,
                                ItemCode = x.ItemCode,
                                ItemName = x.ItemName,
                                UomCode = x.UomCode,
                                Quantity = x.Quantity,
                            })
                            .ToList();

                        await _context.MaintenenceSpareParts
                            .AddRangeAsync(spareParts);
                    }

                    _dataContext.ExecuteNonQuery(@$"UPDATE Equipments SET IsNoti = 1 WHERE ItemCode = '{equipment.ItemCode}'", SqlDbTarget.Default);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OperationResult<string>> CreateItemRequest(CreateItemRequestModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(model.AccountId);

                if (user == null)
                {
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find user");
                }

                var maintenance = await _context.Maintenances
                    .FirstOrDefaultAsync(i => i.Id == model.Id);

                if (maintenance == null)
                {
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find maintenance");
                }

                var oldSpareParts = await _context.MaintenenceSpareParts
                    .Where(x => x.HeaderId == maintenance.Id)
                    .ToListAsync();

                if (oldSpareParts.Any())
                {
                    _context.MaintenenceSpareParts.RemoveRange(oldSpareParts);
                }

                var newSpareParts = model.SparePart
                    .Select(x => new MaintenenceSpareParts
                    {
                        HeaderId = maintenance.Id,
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName,
                        UomCode = x.UomCode,
                        Quantity = x.Quantity
                    })
                    .ToList();

                await _context.MaintenenceSpareParts.AddRangeAsync(newSpareParts);

                var companyService = _oCompany.GetCompanyService();

                var generalService = companyService.GetGeneralService("B1CS_0003");

                var generalData = (GeneralData)generalService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                var children = generalData.Child("ITEM_REQ_D");

                generalData.SetProperty("U_Status", "1");
                generalData.SetProperty("U_RequestDate",DateTime.Now.ToString("yyyy/MM/dd"));
                generalData.SetProperty("U_RequestUser", user.UserName);
                generalData.SetProperty("U_RequestUserName", user.FullName);
                generalData.SetProperty("U_Type", "1");
                generalData.SetProperty("U_Factory", maintenance.FactoryCode);
                generalData.SetProperty("U_WONo", maintenance.DocNo);

                foreach (var item in model.SparePart)
                {
                    var child = children.Add();

                    child.SetProperty("U_ItemCode", item.ItemCode);
                    child.SetProperty("U_Description", item.ItemName);
                    child.SetProperty("U_UOM", item.UomCode);
                    child.SetProperty("U_RequestQty", item.Quantity);
                }

                SAPbobsCOM.GeneralDataParams result;

                try
                {
                    result = generalService.Add(generalData);
                }
                catch (Exception)
                {
                    var sapError = _oCompany.GetLastErrorDescription();

                    await transaction.RollbackAsync();

                    return OperationResult<string>.Fail(ErrorCode.ErrorSap, string.IsNullOrWhiteSpace(sapError) ? "Create item request on SAP failed" : sapError);
                }

                var docEntry = result.GetProperty("DocEntry");

                var maintenanceDoc = new MaintenanceDocs
                {
                    HeaderId = maintenance.Id,
                    DocNo = Convert.ToInt64(docEntry),
                    DocDate = DateTime.Now,
                    DocType = "ItemRequest",
                    Machine = maintenance.ItemCode,
                    CreatedAt = DateTime.Now,
                    Details = model.SparePart.Select(x => new MaintenanceDocDetails
                    {
                        ItemCode = x.ItemCode,
                        Description = x.ItemName,
                        Quantity = x.Quantity,
                        UomCode = x.UomCode
                    }).ToList()
                };

                await _context.MaintenanceDocs.AddAsync(maintenanceDoc);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return OperationResult<string>.Success(message: "Create item request successfully", data: docEntry.ToString() ?? "");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OperationResult<string>> CreatePurchaseRequest(CreatePurchaseRequestModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(model.AccountId);

                if (user == null)
                {
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find user");
                }

                var maintenance = await _context.Maintenances
                    .FirstOrDefaultAsync(i => i.Id == model.Id);

                if (maintenance == null)
                {
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find maintenance");
                }

                var oldSpareParts = await _context.MaintenenceSpareParts
                    .Where(x => x.HeaderId == maintenance.Id)
                    .ToListAsync();

                if (oldSpareParts.Any())
                {
                    _context.MaintenenceSpareParts.RemoveRange(oldSpareParts);
                }

                var newSpareParts = model.SparePart
                    .Select(x => new MaintenenceSpareParts
                    {
                        HeaderId = maintenance.Id,
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName,
                        UomCode = x.UomCode,
                        Quantity = x.Quantity
                    })
                    .ToList();

                await _context.MaintenenceSpareParts.AddRangeAsync(newSpareParts);

                SAPbobsCOM.Documents purchaseRequest = (SAPbobsCOM.Documents)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest);

                purchaseRequest.DocDate = DateTime.Today;
                purchaseRequest.RequriedDate = DateTime.Today;
                purchaseRequest.BPL_IDAssignedToInvoice = 1;

                foreach (var item in model.PurchaseRequest)
                {
                    var whsCode = _context.Equipments.FirstOrDefault(x => x.ItemCode == item.ItemCode)?.FactoryCode;

                    purchaseRequest.Lines.ItemCode = item.ItemCode;
                    purchaseRequest.Lines.ItemDescription = item.ItemName;
                    purchaseRequest.Lines.WarehouseCode = whsCode;
                    purchaseRequest.Lines.Quantity = item.Quantity ?? 0;
                    purchaseRequest.Lines.UoMEntry = GetUomEntryByUomCode(item.UomCode ?? "");

                    purchaseRequest.Lines.Add();
                }

                int sapResult = purchaseRequest.Add();

                if (sapResult != 0)
                {
                    var sapError = _oCompany.GetLastErrorDescription();

                    await transaction.RollbackAsync();

                    return OperationResult<string>.Fail(ErrorCode.ErrorSap, string.IsNullOrWhiteSpace(sapError) ? "Create purchase request on SAP failed": sapError);
                }

                string docEntryString = _oCompany.GetNewObjectKey();

                long docEntry = Convert.ToInt64(docEntryString);

                var pr = (SAPbobsCOM.Documents)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest);

                pr.GetByKey((int)docEntry);

                int docNum = pr.DocNum;

                var maintenanceDoc = new MaintenanceDocs
                {
                    HeaderId = maintenance.Id,
                    DocNo = docNum,
                    DocDate = DateTime.Now,
                    DocType = "PurchaseRequest",
                    ObjectType = (int)SAPbobsCOM.BoObjectTypes.oPurchaseRequest,
                    Machine = maintenance.ItemCode,
                    CreatedAt = DateTime.Now,
                    Details = model.PurchaseRequest.Select(x => new MaintenanceDocDetails
                    {
                        ItemCode = x.ItemCode,
                        Description = x.ItemName,
                        Quantity = x.Quantity,
                        UomCode = x.UomCode
                    }).ToList()
                };

                await _context.MaintenanceDocs.AddAsync(maintenanceDoc);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return OperationResult<string>.Success(message: "Create item purchase request successfully", data: docNum.ToString() ?? "");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OperationResult<string>> CreatePurchaseRequestService(CreatePurchaseRequestServiceModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(model.AccountId);

                if (user == null)
                {
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find user");
                }

                var maintenance = await _context.Maintenances.FirstOrDefaultAsync(i => i.Id == model.Id);

                if (maintenance == null)
                {
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find maintenance");
                }

                SAPbobsCOM.Documents purchaseRequest = (SAPbobsCOM.Documents)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest);

                purchaseRequest.DocDate = DateTime.Today;
                purchaseRequest.RequriedDate = DateTime.Today;
                purchaseRequest.BPL_IDAssignedToInvoice = 1;
                purchaseRequest.Comments = model.Content;

                purchaseRequest.Lines.ItemCode = "90000058";
                purchaseRequest.Lines.WarehouseCode = "GO";
                purchaseRequest.Lines.Quantity = 1;
                purchaseRequest.Lines.UoMEntry = 77;
                purchaseRequest.Lines.RequiredDate = model.TimeRequiredService;

                int sapResult = purchaseRequest.Add();

                if (sapResult != 0)
                {
                    var sapError = _oCompany.GetLastErrorDescription();

                    await transaction.RollbackAsync();

                    return OperationResult<string>.Fail(ErrorCode.ErrorSap, string.IsNullOrWhiteSpace(sapError) ? "Create purchase request on SAP failed" : sapError);
                }

                string docEntryString = _oCompany.GetNewObjectKey();

                long docEntry = Convert.ToInt64(docEntryString);

                var pr = (SAPbobsCOM.Documents)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseRequest);

                pr.GetByKey((int)docEntry);

                int docNum = pr.DocNum;

                var maintenanceDoc = new MaintenanceDocs
                {
                    HeaderId = maintenance.Id,
                    DocNo = docNum,
                    DocDate = DateTime.Now,
                    DocType = "PurchaseRequestService",
                    ObjectType = (int)SAPbobsCOM.BoObjectTypes.oPurchaseRequest,
                    Machine = maintenance.ItemCode,
                    CreatedAt = DateTime.Now,
                    Details =
                    [
                        new MaintenanceDocDetails
                        {
                            ItemCode = "90000058",
                            Description = "Chi phí sửa chữa thiết bị tại nhà máy - BP.Sản xuất",
                            Quantity = 1
                        }
                    ]
                };

                await _context.MaintenanceDocs.AddAsync(maintenanceDoc);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return OperationResult<string>.Success(message: "Create item purchase request successfully", data: docNum.ToString() ?? "");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OperationResult<string>> UpdateStatusMaintenance(UpdateMaintenanceStatusModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var maintenance = await _context.Maintenances.FirstOrDefaultAsync(m => m.Id == model.Id);

                if (maintenance == null)
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find maintenance");
                
                var user = await _userManager.FindByIdAsync(model.AccountId);

                if (user == null)
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find user");

                maintenance.Status = model.Status;
                maintenance.Remark = model.Remark; 
                maintenance.UpdatedBy = user.UserName;
                maintenance.UpdatedDate = DateTime.Now;

                if(model.Status == 2)
                {
                    if (model.Attachments?.Any() == true)
                    {
                        var uploadFolder = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "Upload",
                            "Maintenance",
                            maintenance.Id.ToString());

                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        var attachmentEntities = new List<MaintenenceAttachments>();

                        foreach (var attachment in model.Attachments)
                        {
                            if (attachment.File == null ||
                                attachment.File.Length == 0)
                            {
                                continue;
                            }

                            var extension = Path.GetExtension(
                                attachment.File.FileName);

                            var fileName =
                                $"{Guid.NewGuid():N}{extension}";

                            var fullPath =
                                Path.Combine(uploadFolder, fileName);

                            await using (var stream = new FileStream(
                                fullPath,
                                FileMode.Create))
                            {
                                await attachment.File.CopyToAsync(stream);
                            }

                            var relativePath =
                                $"/Upload/Maintenance/{maintenance.Id}/{fileName}";

                            var fileType =
                                attachment.File.ContentType.StartsWith("video/")
                                    ? "video"
                                    : "image";

                            attachmentEntities.Add(
                                new MaintenenceAttachments
                                {
                                    HeaderId = maintenance.Id,
                                    Type = 1,
                                    FilePath = relativePath,
                                    FileName = attachment.File.FileName,
                                    FileType = fileType,
                                    Order = attachment.Order
                                });
                        }

                        if (attachmentEntities.Any())
                        {
                            _context.MaintenenceAttachments.AddRange(
                                attachmentEntities);
                        }
                    }

                    maintenance.DueDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return OperationResult<string>.Success(message: "Update maintenance successfully", data: maintenance.DocNo);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OperationResult<string>> SaveCheckList(SaveCheckListModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var maintenance = await _context.Maintenances.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.MaintenanceId);

                if (maintenance == null)
                {
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find maintenance");
                }

                if (model.IsDeleted)
                {
                    var entity = await _context.MaintenenceCheckLists.FirstOrDefaultAsync(x => x.Id == model.Id && x.HeaderId == model.MaintenanceId);

                    if (entity == null)
                    {
                        return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find checklist");
                    }

                    _context.MaintenenceCheckLists.Remove(entity);
                }
                else if (!model.Id.HasValue)
                {
                    var entity = new MaintenenceCheckLists
                    {
                        HeaderId = model.MaintenanceId,
                        JobType = model.JobType,
                        CheckItem = model.CheckItem,
                        EvaluationStandard = model.EvaluationStandard,
                        DataType = null,
                        CheckResult = model.CheckResult,
                        EquipmentStatus = model.EquipmentStatus,
                        Remark = model.Remark
                    };

                    await _context.MaintenenceCheckLists.AddAsync(entity);
                }
                else
                {
                    var entity = await _context.MaintenenceCheckLists
                        .FirstOrDefaultAsync(x =>
                            x.Id == model.Id &&
                            x.HeaderId == model.MaintenanceId);

                    if (entity == null)
                    {
                        return OperationResult<string>.Fail(ErrorCode.NotFound, "Can not find checklist");
                    }

                    entity.JobType = model.JobType;
                    entity.CheckItem = model.CheckItem;
                    entity.EvaluationStandard = model.EvaluationStandard;
                    entity.CheckResult = model.CheckResult;
                    entity.EquipmentStatus = model.EquipmentStatus;
                    entity.Remark = model.Remark;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return OperationResult<string>.Success(message: "Update checklist successfully", data: maintenance.DocNo);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public int GetUomEntryByUomCode(string uomCode)
        {
            string query = "SELECT UomEntry FROM OUOM WHERE UomCode = @UomCode";

            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@UomCode", SqlDbType.NVarChar) { Value = uomCode }
            };

            int uomEntry = _dataContext.ExecuteQuery<int>(query, SqlDbTarget.HaiHaCo, parameters).FirstOrDefault();

            return uomEntry;
        }
    }
}