using AutoMapper;
using LinqToDB.Async;
using Maintenance.Entities.Maintenance;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Common;
using Maintenance.Infrastructure.SqlServer.Data;
using Maintenance.Infrastructure.SqlServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
        private readonly DataContextSql _dataContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public MaintenanceRepository(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration, DataContextSql dataContextSql)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _dataContext = dataContextSql;
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
                string query = "B1CS_GET_ALL_MAINTENANCE_TECHNICAL";

                var parameters = new[]
                {
                    new SqlParameter("@AccountId", SqlDbType.NVarChar)
                    {
                        Value = maintenanceCreterias.AccountId,
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
                    DueDate = model.PlannedCompletionDate ?? model.DocDate,
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
    }
}
