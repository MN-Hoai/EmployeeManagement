using DBContext.EmployeeMangement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EmployeeMangement.Executes
{
    public class JobPositionCommand
    {

        private readonly EmployeeManagementContext _context;

        public JobPositionCommand(EmployeeManagementContext context)
        {
            _context = context;
        }
        public async Task<bool> DeleteJobPositionById(int? id)
        {
            var jobPosition = await _context.JobPositions.FirstOrDefaultAsync(a => a.Id == id);
            if (jobPosition == null)
                return false;

            jobPosition.Status = -1;
            _context.JobPositions.Update(jobPosition);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

     
