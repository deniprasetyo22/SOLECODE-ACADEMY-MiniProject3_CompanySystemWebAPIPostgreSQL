using MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Interfaces
{
    public interface IWorksOn
    {
        Task<IEnumerable<Workson>> GetAllWorkson(int pageNumber, int pageSize);
        Task<Workson?> GetWorksonById(int empNo, int projNo);
        Task<bool> AddWorkson(Workson worksOn);
        Task<bool> UpdateWorkson(int empNo, int projNo, Workson editWorksOn);
        Task<bool> DeleteWorkson(int empNo, int projNo);
    }
}
