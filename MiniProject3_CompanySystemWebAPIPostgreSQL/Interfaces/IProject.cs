using MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Interfaces
{
    public interface IProject
    {
        Task<bool> AddProject(Project project);
        Task<IEnumerable<Project>> GetAllProjects(int pageNumber, int pageSize);
        Task<Project> GetProjectById(int projNo);
        Task<bool> UpdateProject(int projNo, Project editProj);
        Task<bool> DeleteProject(int projNo);
        Task<IEnumerable<Project>> GetProjectsManagedByDept();
        Task<IEnumerable<Project>> GetProjectsManagedByDepartments(params string[] departmentNames);
        Task<IEnumerable<Project>> GetProjectsWithNoEmployees();
    }
}
