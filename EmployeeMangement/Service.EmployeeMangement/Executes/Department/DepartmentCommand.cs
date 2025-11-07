using DBContext.EmployeeMangement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EmployeeMangement.Executes
{
    public class DepartmentCommand
    {
        private readonly EmployeeManagementContext _context;

        public DepartmentCommand(EmployeeManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Xóa 1 hoặc nhiều bài viết
        /// </summary>
        public async Task<bool> DeleteDepartmentById(int? id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(a => a.Id == id);
            if (department == null)
                return false;

            department.Status = -1;
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
