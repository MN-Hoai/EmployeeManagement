using DBContext.EmployeeMangement;
using DBContext.EmployeeMangement.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.EmployeeMangement.Executes.EmployeeManyModel;

namespace Service.EmployeeMangement.Executes
{
    public class EmployeeOne
    {
        private readonly EmployeeManagementContext _context;
        public EmployeeOne(EmployeeManagementContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeResponse>> GetEmployee(int? id = 0, string? email = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new List<EmployeeResponse>();
            IQueryable<Employee> query = _context.Employees
                .Include(e => e.JobPosition)
                .Include(e => e.Department)
                .Where(e => e.Status == 1);

            if (id != 0)
            {
                query = query.Where(e => e.Id == id);
            }
            if (email != null)
            {
                query = query.Where(e => e.Email == email);
            }
            var result = await query
              .Select(e => new EmployeeResponse
              {
                  Id = e.Id,
                  Keyword = e.Keyword,
                  Status = e.Status,
                  CreateBy = e.CreateBy,
                  CreateDate = e.CreateDate,
                  UpdatedBy = e.UpdatedBy,
                  UpdatedDate = e.UpdatedDate,
                  Fullname = e.Fullname,
                  Email = e.Email,
                  Phone = e.Phone,
                  Position = e.Position,
                  Address = e.JobPosition.Address,
                  JobPositionCode = e.JobPosition != null ? e.JobPosition.Code : null,
                  JobPositionName = e.JobPosition != null ? e.JobPosition.Name : null,
                  DepartmentName = e.Department != null ? e.Department.Name : null,
                  DepartmentCode = e.Department != null ? e.Department.Code : null
              })
                .ToListAsync();

            return result;
        }
    }
}
