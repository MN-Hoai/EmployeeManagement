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
    public class EmployeeCommand
    {
        private readonly EmployeeManagementContext _context;

        public EmployeeCommand(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<int> Delete(int id)
        {
            var items = await _context.Employees
                .FirstOrDefaultAsync(p => p.Id == id);

            if (items == null)
                return 0; 

            items.Status = -1; 

            _context.Employees.Update(items);
            return await _context.SaveChangesAsync();
        }
    }
}
