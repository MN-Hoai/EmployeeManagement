using DBContext.EmployeeMangement;
using DBContext.EmployeeMangement.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EmployeeMangement.Executes
{
    public class JobPositionOne
    {
        private readonly EmployeeManagementContext _context;
        public JobPositionOne(EmployeeManagementContext context)
        {
            _context = context;
        }
        public async Task<Department?> GetJobPositionById(int id)
        {
            var department = await _context.Departments.
                Include(d => d.Manager).ThenInclude(m => m.Media).
                Include(d => d.CreateByNavigation).
                Include(d => d.UpdateByNavigation).
                Include(p => p.JobPosition).FirstOrDefaultAsync(d => d.Id == id && d.Status != -1);
            return department;
        }
    }
}
