using DBContext.EmployeeMangement;
using DBContext.EmployeeMangement.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Service.EmployeeMangement.Executes.JobPositionModel;

namespace Service.EmployeeMangement.Executes
{
    public class JobPositionMany
    {
        private readonly EmployeeManagementContext _context;
        public JobPositionMany(EmployeeManagementContext context)
        {
            _context = context;
        }
        public async Task<List<JobPosition>> GetDepartments()
        {
            return await _context.JobPositions
                                 .Where(d => d.Status == 1)
                                 .ToListAsync();
        }
        public async Task<List<JobPositionResponse>> GetAllJobPositionName()
        {
            return await _context.JobPositions
                .Where(d => d.Status == 1)
                .Select(d => new JobPositionResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Address = d.Address,
                })
                .ToListAsync();
        }
    }
}
