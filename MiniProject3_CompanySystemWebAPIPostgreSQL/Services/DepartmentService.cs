using Microsoft.EntityFrameworkCore;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Interfaces;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Services
{
    public class DepartmentService : IDepartment
    {
        private readonly CompanyContext _context;
        public DepartmentService(CompanyContext context)
        {
            _context = context;
        }
        private async Task<int> GenerateNextDeptNo()
        {
            var lastDeptNo = await _context.Departments
                .OrderByDescending(d => d.Deptno)
                .Select(d => d.Deptno)
                .FirstOrDefaultAsync();
            return lastDeptNo + 1;
        }
        public async Task<bool> AddDepartment(Department department)
        {
            if (department.Deptno == 0)
            {
                department.Deptno = await GenerateNextDeptNo();
            }

            // Validasi apakah Mgrempno ada di database
            var managerExists = await _context.Employees.AnyAsync(e => e.Empno == department.Mgrempno);
            if (!managerExists)
            {
                return false;
            }

            var existingDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Deptname == department.Deptname || d.Mgrempno == department.Mgrempno);
            if (existingDepartment != null)
            {
                return false;
            }

            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Department>> GetAllDepartments(int pageNumber, int pageSize)
        {
            return await _context.Departments
                .OrderBy(d => d.Deptno)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Department> GetDepartmentById(int deptNo)
        {
            if (deptNo <= 0)
            {
                throw new ArgumentException("Invalid department number.", nameof(deptNo));
            }
            var filtered = await _context.Departments.FirstOrDefaultAsync(cek => cek.Deptno == deptNo);
            return filtered;
        }
        public async Task<bool> UpdateDepartment(int deptNo, Department editDept)
        {
            if (deptNo <= 0 || editDept == null)
            {
                throw new ArgumentException("Invalid department number or data.");
            }
            var existingDepartment = await _context.Departments.FirstOrDefaultAsync(cek => cek.Deptno == deptNo);
            if (existingDepartment == null)
            {
                return false;
            }
            // Cek apakah Mgrempno sudah ada di database dan berbeda dari yang sedang diupdate
            var managerExists = await _context.Departments.AnyAsync(cek => cek.Mgrempno == editDept.Mgrempno && cek.Deptno != deptNo);

            if (managerExists)
            {
                // Jika Mgrempno sudah ada dan berbeda dari deptNo yang sedang diupdate, return false
                return false;
            }
            existingDepartment.Deptname = editDept.Deptname;
            existingDepartment.Mgrempno = editDept.Mgrempno;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteDepartment(int deptNo)
        {
            if (deptNo <= 0)
            {
                throw new ArgumentException("Invalid department number.", nameof(deptNo));
            }
            var department = await _context.Departments.FirstOrDefaultAsync(cek => cek.Deptno == deptNo);
            if (department == null)
            {
                return false;
            }
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
