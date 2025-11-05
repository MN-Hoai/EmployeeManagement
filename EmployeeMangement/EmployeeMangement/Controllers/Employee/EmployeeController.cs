using EmployeeMangement.Controllers.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.EmployeeMangement.Executes;
using System.Security.Claims;
using static Service.EmployeeMangement.Executes.DepartmentModel;
using static Service.EmployeeMangement.Executes.EmployeeModel;
using static Service.EmployeeMangement.Executes.JobPositionModel;

namespace EmployeeMangement.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly EmployeeOne _employeeOne;
        private readonly EmployeeMany _employeeMany;
        private readonly DepartmentMany _departmentMany;
        private readonly EmployeeCommand _employeeCommand;
        private readonly JobPositionMany _jobPositionMany;


        public EmployeeController(EmployeeOne employeeOne, EmployeeMany employeeMany,
            EmployeeCommand employeeCommand, DepartmentMany departmentMany, JobPositionMany jobPositionMany)
        {
            _employeeOne = employeeOne;
            _employeeMany = employeeMany;
            _employeeCommand = employeeCommand;
            _departmentMany = departmentMany;
            _jobPositionMany = jobPositionMany;
        }

        public IActionResult List()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claims = User.Identity as ClaimsIdentity;
                ViewBag.Username = claims?.FindFirst(ClaimTypes.Name)?.Value;
                ViewBag.AccountId = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ViewBag.Email = claims?.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.Name = claims?.FindFirst(ClaimTypes.Name)?.Value;
            }
            else
            {
                ViewBag.Username = "";
                ViewBag.AccountId = "";
                ViewBag.Email = "";
                ViewBag.Name = "";
            }

            return View();
        }


        public IActionResult Header()
        {
            return PartialView();
        }
        public async Task<IActionResult> EmployeeList()
        {
            return PartialView("~/Views/Shared/Page/_EmployeeList.cshtml");
        }
        public async Task<IActionResult> AddEmployee()
        {
            var model = new EmployeeResponse();
            var departments = await _departmentMany.GetAllDepartmentName();

            model.Departments = departments.Select(x => new DepartmentResponse
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            var jobpositions = await _jobPositionMany.GetAllJobPositionName();
            model.JobPositions = jobpositions.Select(y => new JobPositionResponse
            {
                Id = y.Id,
                Name = y.Name,
                Address = y.Address,

            }).ToList();
            return PartialView("~/Views/Shared/Page/_EditAddEmployee.cshtml", model);
        }

        // GET: api/employees
        [HttpGet("api/employees")]
        public async Task<IActionResult> GetAll(FilterListRequest filter)
        {

            if (filter == null) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - dữ liệu rỗng" }); }

            var isValid = SqlGuard.IsSuspicious(filter);
            if (isValid) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - model không hợp lệ" }); }
            try
            {
                var result = await _employeeMany.Gets(filter);
                if (result == null) { return NotFound(new { succsess = false, message = "Không có dữ liệu" }); }
                return Ok(new
                {
                    succsess = result,
                    message = "Lấy dữ liệu thành công"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { succsess = false, message = "Không thể kết nối server" });

            }
        }

        // GET: api/employees/5
        [HttpGet("api/employee/{id:int}/{mode}")]
        public async Task<IActionResult> GetById(int id = 0, string mode = "view")
        {
            if (id == 0) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - dữ liệu rỗng" }); }

            var isValid = SqlGuard.IsSuspicious(id);
            if (isValid) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - id không hợp lệ" }); }
            try
            {
                var result = await _employeeOne.Get(id, null);
                if (result == null || !result.Any()) { return NotFound(new { succsess = false, message = "Không có dữ liệu" }); }


                var employee = result.FirstOrDefault();
                if (employee == null) { return NotFound(new { succsess = false, message = "Không có dữ liệu" }); }

                var model = new EmployeeResponse()
                {
                    Id = employee.Id,
                    Keyword = employee.Keyword,
                    Fullname = employee.Fullname,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    Position = employee.Position,
                    Status = employee.Status,
                    CreateBy = employee.CreateBy,
                    CreateByName = employee.CreateByName,
                    UpdatedByName = employee.UpdatedByName,
                    CreateDate = employee.CreateDate,
                    UpdatedBy = employee.UpdatedBy,
                    UpdatedDate = employee.UpdatedDate,
                    JobPositionName = employee.JobPositionName,
                    JobPositionId = employee.JobPositionId,
                    DepartmentName = employee.DepartmentName,
                    DepartmentId = employee.DepartmentId,
                    Address = employee.Address,
                  
                };
                if (mode == "view")
                {
                    return PartialView("~/Views/Shared/Page/_ViewDetailEmployee.cshtml", model);
                }
                var departments = await _departmentMany.GetAllDepartmentName();

                model.Departments = departments.Select(x => new DepartmentResponse
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
                var jobpositions = await _jobPositionMany.GetAllJobPositionName();
                model.JobPositions = jobpositions.Select(y => new JobPositionResponse
                {
                    Id = y.Id,
                    Name = y.Name,

                }).ToList();
                return PartialView("~/Views/Shared/Page/_EditAddEmployee.cshtml", model);







            }
            catch (Exception)
            {
                return StatusCode(500, new { succsess = false, message = "Không thể kết nối server" });
            }
        }

        // GET: api/employees/5
        [HttpGet("api/employee/email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            if (email.IsNullOrEmpty()) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - dữ liệu rỗng" }); }

            var isValid = SqlGuard.IsSuspicious(email);
            if (isValid) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - email không hợp lệ" }); }
            try
            {
                var result = await _employeeOne.Get(0, email);
                if (result == null) { return NotFound(new { succsess = false, message = "Không có dữ liệu" }); }
                return Ok(new
                {
                    succsess = result,
                    message = "Lấy dữ liệu thành công"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { succsess = false, message = "Không thể kết nối server" });

            }
        }
        // POST: api/employees
        [HttpPost("api/employee/create")]
        public async Task<IActionResult> Create([FromBody] EmployeeResponse request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ - dữ liệu rỗng" });

            if (SqlGuard.IsSuspicious(request))
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
            try
            {
                var claims = User.Identity as ClaimsIdentity;
                var accountIdString = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(accountIdString, out int accountId) || accountId <= 0)
                {
                    return Unauthorized(new { success = false, message = "Không xác thực được tài khoản" });
                }
                var result = await _employeeCommand.Create(request, accountId);
                if (result == 0)
                    return NotFound(new { success = false, message = "Không tìm thấy dữ liệu để cập nhật" });

                if (result == -1)
                    return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật dữ liệu" });

                return Ok(new
                {
                    success = true,
                    message = "Lưu dữ liệu thành công"
                });
            }
            catch (Exception)
            {

                return StatusCode(500, new { success = false, message = "Không thể kết nối server" });
            }
        }


        [HttpPut("api/employee/update")]
        public async Task<IActionResult> Update([FromBody] EmployeeResponse request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ - dữ liệu rỗng" });

            if (request.Id <= 0)
                return BadRequest(new { success = false, message = "Mã nhân viên không hợp lệ" });

            if (SqlGuard.IsSuspicious(request))
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

            try
            {
                var claims = User.Identity as ClaimsIdentity;
                var accountIdString = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(accountIdString, out int accountId) || accountId <= 0)
                {
                    return Unauthorized(new { success = false, message = "Không xác thực được tài khoản" });
                }

                var result = await _employeeCommand.Update(request, accountId);

                if (result == 0)
                    return NotFound(new { success = false, message = "Không tìm thấy dữ liệu để cập nhật" });

                if (result == -1)
                    return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật dữ liệu" });

                return Ok(new
                {
                    success = true,
                    message = "Lưu dữ liệu thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Không thể kết nối server: {ex.Message}" });
            }
        }


        //// DELETE: api/employee/delete/5
        [HttpPost("api/employee/delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - dữ liệu rỗng" }); }

            var isValid = SqlGuard.IsSuspicious(id);
            if (isValid) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - id không hợp lệ" }); }
            try
            {
                var result = await _employeeCommand.Delete(id);
                if (result == 0) { return NotFound(new { succsess = false, message = "Không có dữ liệu" }); }
                return Ok(new
                {
                    success = true,
                    message = "Xóa dữ liệu thành công"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { succsess = false, message = "Không thể kết nối server" });

            }
        }
    }
}
