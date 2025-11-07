using DBContext.EmployeeMangement;
using DBContext.EmployeeMangement.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Service.EmployeeMangement
{
    public class DepartmentOne
    {
        private readonly EmployeeManagementContext _context;

        public DepartmentOne(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<Department?> GetDepartmentById(int id)
        {
            var department = await _context.Departments.
                Include(d => d.Manager).ThenInclude(m => m.Media).
                Include(d => d.CreateByNavigation).
                Include(d => d.UpdateByNavigation).
                Include(p => p.JobPosition).FirstOrDefaultAsync(d => d.Id == id && d.Status != -1);
            return department;
        }
        





























        //public async Task<Department?> GetDepartmentById(int id)
        //{
        //    var department = await _context.Departments
        //        .Include(d => d.Manager)
        //            .ThenInclude(m => m.Media)  
        //        .Include(d => d.JobPosition)  
        //        .FirstOrDefaultAsync(d => d.Id == id && d.Status != -1);

        //    return department;
        //}
    }
}
