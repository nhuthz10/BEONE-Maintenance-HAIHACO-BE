using Maintenance.Entities.Factory;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Repositories.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.FactoryUseCase
{
    public class FactoryService : IFactoryService
    {
        private readonly IFactoryRepository _factoryRepository;
        public FactoryService(IFactoryRepository factoryRepository) 
        {
            _factoryRepository = factoryRepository;
        }

        public async Task<OperationResult<List<FactoryViewModel>>> GetAllFactory()
        {
            try
            {
                var result = await _factoryRepository.GetAllFactory();
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
