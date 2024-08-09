using MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Interfaces
{
    public interface IDepartment
    {
        Task<bool> AddDepartment(Department department);
        Task<IEnumerable<Department>> GetAllDepartments(int pageNumber, int pageSize);
        Task<Department> GetDepartmentById(int dptNo);
        Task<bool> UpdateDepartment(int deptNo, Department editDept);
        Task<bool> DeleteDepartment(int deptNo);
    }
}
