using MiniProject3_CompanySystemWebAPIPostgreSQL.Models;
using System.Threading.Tasks;
using static MiniProject3_CompanySystemWebAPIPostgreSQL.Services.EmployeeService;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Interfaces
{
    public interface IEmployee
    {
        Task<bool> AddEmployee(Employee employee);
        Task<IEnumerable<Employee>> GetAllEmployees(int pageNumber, int pageSize);
        Task<Employee> GetEmployeeById(int empNo);
        Task<bool> UpdateEmployee(int empNo, Employee editEmp);
        Task<bool> DeleteEmployee(int empNo);
        Task<IEnumerable<Employee>> GetEmployeesFromBRICS();
        Task<IEnumerable<Employee>> GetEmployeesBornBetween1980And1990();
        Task<IEnumerable<Employee>> GetFemaleEmployeesBornAfter1990();
        Task<IEnumerable<Employee>> GetFemaleManagers();
        Task<IEnumerable<Employee>> GetEmployeesNotManagers();
        Task<IEnumerable<DepartmentEmployeeCount>> GetDepartmentsWithMoreThanTenEmployees();

    }
}
