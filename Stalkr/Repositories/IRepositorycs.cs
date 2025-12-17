using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Neo4j.Driver;

namespace Stalkr.Repositories
{
    public interface IRepository <TModel>
    {
        Task<IEnumerable<TModel>> GetAllAsync();
        Task<TModel?> FindByIdAsync(int id);
        Task<bool> InsertAsync(TModel dto);
        Task<bool> UpdateAsync(int id, TModel dto);
        Task<bool> DeleteAsync(int id);
    }
}
