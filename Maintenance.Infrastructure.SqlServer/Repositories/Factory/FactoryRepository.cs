using AutoMapper;
using Maintenance.Entities.Equipment;
using Maintenance.Entities.Factory;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Common;
using Maintenance.Infrastructure.SqlServer.Data;

namespace Maintenance.Infrastructure.SqlServer.Repositories.Factory
{
    public class FactoryRepository : IFactoryRepository
    {
        private readonly DataContextSql _dataContext;
        private readonly IMapper _mapper;

        public FactoryRepository(IMapper mapper, DataContextSql dataContextSql)
        {
            _mapper = mapper;
            _dataContext = dataContextSql;
        }

        public async Task<OperationResult<List<FactoryViewModel>>> GetAllFactory()
        {
            try
            {
                string query = "B1CS_GET_FACTORY";

                var dataRows = _dataContext.ExecuteStoredProcedureRaw(query, DataContextSql.SqlDbTarget.HaiHaCo);

                var result = dataRows
                .Select(p => new FactoryViewModel
                {
                    FactoryCode = p.GetString("FactoryCode"),
                    FactoryName = p.GetString("FactoryName"),
                })
                .ToList();

                return OperationResult<List<FactoryViewModel>>.Success(message: "Get all factory successfully", data: result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
