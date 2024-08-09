using Microsoft.EntityFrameworkCore;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Interfaces;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Services
{
    public class WorksonService : IWorksOn
    {
        private readonly CompanyContext _context;

        public WorksonService(CompanyContext context)
        {
            _context = context;
        }

        public async Task<bool> AddWorkson(Workson worksOn)
        {
            // Validasi apakah Workson sudah ada
            var existingWorkson = await _context.Worksons
                .FirstOrDefaultAsync(w => w.Empno == worksOn.Empno && w.Projno == worksOn.Projno);
            if (existingWorkson != null)
            {
                return false;
            }

            // Validasi keberadaan Employee dan Project
            var employeeExists = await _context.Employees.AnyAsync(e => e.Empno == worksOn.Empno);
            var projectExists = await _context.Projects.AnyAsync(p => p.Projno == worksOn.Projno);
            if (!employeeExists || !projectExists)
            {
                return false;
            }

            _context.Worksons.Add(worksOn);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Workson>> GetAllWorkson(int pageNumber, int pageSize)
        {
            return await _context.Worksons
                .OrderBy(w => w.Empno)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Workson?> GetWorksonById(int empNo, int projNo)
        {
            return await _context.Worksons.FirstOrDefaultAsync(w => w.Empno == empNo && w.Projno == projNo);
        }


        public async Task<bool> UpdateWorkson(int empNo, int projNo, Workson editWorksOn)
        {
            var existingWorkson = await _context.Worksons
            .FirstOrDefaultAsync(w => w.Empno == empNo && w.Projno == projNo);
            if (existingWorkson == null)
            {
                return false;
            }

            // Validasi keberadaan Employee dan Project
            var employeeExists = await _context.Employees.AnyAsync(e => e.Empno == editWorksOn.Empno);
            var projectExists = await _context.Projects.AnyAsync(p => p.Projno == editWorksOn.Projno);
            if (!employeeExists || !projectExists)
            {
                return false;
            }

            existingWorkson.Dateworked = editWorksOn.Dateworked;
            existingWorkson.Hoursworked = editWorksOn.Hoursworked;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWorkson(int empNo, int projNo)
        {
            var existingWorkson = await _context.Worksons.FirstOrDefaultAsync(w => w.Empno == empNo || w.Projno == projNo);
            if (existingWorkson == null)
            {
                return false;
            }

            _context.Worksons.Remove(existingWorkson);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
