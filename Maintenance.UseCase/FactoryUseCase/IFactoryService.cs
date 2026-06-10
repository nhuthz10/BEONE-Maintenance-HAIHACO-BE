using Maintenance.Entities.Factory;
using Maintenance.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.FactoryUseCase
{
    public interface IFactoryService
    {
        public Task<OperationResult<List<FactoryViewModel>>> GetAllFactory();
    }
}
