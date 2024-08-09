using Microsoft.EntityFrameworkCore;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Interfaces;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Services
{
    public class EmployeeService : IEmployee
    {
        private readonly CompanyContext _context;
        public EmployeeService(CompanyContext context)
        {
            _context = context;
        }
        private async Task<int> GenerateNextEmpNo()
        {
            var lastEmpNo = await _context.Employees
                .OrderByDescending(e => e.Empno)
                .Select(e => e.Empno)
                .FirstOrDefaultAsync();
            return lastEmpNo + 1;
        }
        public async Task<bool> AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee data cannot be null.");
            }

            if (employee.Empno == 0)
            {
                employee.Empno = await GenerateNextEmpNo();
            }

            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(cek => cek.Empno == employee.Empno);
            if (existingEmployee != null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(employee.Fname) ||
                string.IsNullOrWhiteSpace(employee.Lname) ||
                string.IsNullOrWhiteSpace(employee.Position) ||
                employee.Deptno == 0)
            {
                return false;
            }
            var existingDepartment = await _context.Departments
                .FirstOrDefaultAsync(dep => dep.Deptno == employee.Deptno);
            if (existingDepartment == null)
            {
                return false;
            }

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployees(int pageNumber, int pageSize)
        {
            return await _context.Employees
                .OrderBy(a => a.Empno)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Employee> GetEmployeeById(int empNo)
        {
            var filtered = await _context.Employees.FirstOrDefaultAsync(cek => cek.Empno == empNo);
            return filtered;
        }
        public async Task<bool> UpdateEmployee(int empNo, Employee editEmp)
        {
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(cek => cek.Empno == empNo);
            if (existingEmployee == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(editEmp.Fname) || string.IsNullOrEmpty(editEmp.Lname) ||
                string.IsNullOrEmpty(editEmp.Position) || editEmp.Deptno == 0)
            {
                return false;
            }

            var existingDepartment = await _context.Departments.FirstOrDefaultAsync(dep => dep.Deptno == editEmp.Deptno);
            if (existingDepartment == null)
            {
                return false;
            }
            existingEmployee.Fname = editEmp.Fname;
            existingEmployee.Lname = editEmp.Lname;
            existingEmployee.Address = editEmp.Address;
            existingEmployee.Dob = editEmp.Dob;
            existingEmployee.Sex = editEmp.Sex;
            existingEmployee.Position = editEmp.Position;
            existingEmployee.Deptno = editEmp.Deptno;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteEmployee(int empNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(cek => cek.Empno == empNo);
            if (employee == null)
            {
                return false;
            }
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Employee>> GetEmployeesFromBRICS()
        {
            var bricsCountries = new List<string> { "Brazil", "Russia", "India", "China", "South Africa" };
            var employeesFromBRICS = await _context.Employees
                .Where(cek => bricsCountries
                .Contains(cek.Address))
                .OrderBy(a => a.Address)
                .ToListAsync();
            return employeesFromBRICS;
        }
        public async Task<IEnumerable<Employee>> GetEmployeesBornBetween1980And1990()
        {
            var employeesBornBetween1980And1990 = await _context.Employees
                .Where(cek => cek.Dob >= new DateOnly(1980, 1, 1) && cek.Dob <= new DateOnly(1990, 12, 31))
                .OrderBy(a => a.Empno)
                .ToListAsync();
            return employeesBornBetween1980And1990;
        }
        public async Task<IEnumerable<Employee>> GetFemaleEmployeesBornAfter1990()
        {
            var femaleEmployeesBornAfter1990 = await _context.Employees
                .Where(cek => cek.Sex == "Female" && cek.Dob > new DateOnly(1990, 12, 31))
                .ToListAsync();

            return femaleEmployeesBornAfter1990;
        }
        public async Task<IEnumerable<Employee>> GetFemaleManagers()
        {
            var femaleManagers = await _context.Employees
                .Where(cek => cek.Sex == "Female" && _context.Departments.Any(a => a.Mgrempno == cek.Empno))
                .OrderBy(b => b.Lname)
                .ThenBy(c => c.Fname)
                .ToListAsync();

            return femaleManagers;
        }
        public async Task<IEnumerable<Employee>> GetEmployeesNotManagers()
        {
            var employeesNotManagers = await _context.Employees
                .Where(cek => !_context.Departments.Any(a => a.Mgrempno == cek.Empno))
                .ToListAsync();

            return employeesNotManagers;
        }
        public async Task<int> GetNumberOfFemaleManagers()
        {
            var count = await _context.Employees
                .Where(e => e.Position == "HR Manager" && e.Sex == "Female")
                .CountAsync();

            return count;
        }

        public async Task<IEnumerable<DepartmentEmployeeCount>> GetDepartmentsWithMoreThanTenEmployees()
        {
            var departmentEmployeeCounts = await _context.Employees
                .GroupBy(e => e.Deptno)
                .Select(g => new
                {
                    Deptno = g.Key,
                    EmployeeCount = g.Count()
                })
                .Where(dc => dc.EmployeeCount > 10)
                .Join(_context.Departments,
                    dc => dc.Deptno,
                    d => d.Deptno,
                    (dc, d) => new DepartmentEmployeeCount
                    {
                        Deptno = d.Deptno,
                        Deptname = d.Deptname,
                        EmployeeCount = dc.EmployeeCount
                    })
                .ToListAsync();

            return departmentEmployeeCounts;
        }
        public class DepartmentEmployeeCount
        {
            public int Deptno { get; set; }
            public string Deptname { get; set; }
            public int EmployeeCount { get; set; }
        }

    }
}
