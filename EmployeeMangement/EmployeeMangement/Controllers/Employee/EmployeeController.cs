using EmployeeMangement.Controllers.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.EmployeeMangement.Executes;
using System.Security.Claims;
using static Service.EmployeeMangement.Executes.EmployeeModel;

namespace EmployeeMangement.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly EmployeeOne _employeeOne;
        private readonly EmployeeMany _employeeMany;
        private readonly EmployeeCommand _employeeCommand;
        public EmployeeController(EmployeeOne employeeOne, EmployeeMany employeeMany, EmployeeCommand employeeCommand)
        {
            _employeeOne = employeeOne;
            _employeeMany = employeeMany;
            _employeeCommand = employeeCommand;
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
            return PartialView("~/Views/Shared/Page/_EditAddEmployee.cshtml");
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
        [HttpGet("api/employee/{id:int}")]
        public async Task<IActionResult> GetById(int id = 0)
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
                    Fullname = employee.Fullname,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    Position = employee.Position,
                    Status = employee.Status,
                    CreateBy = employee.CreateBy,
                    CreateByName = employee.CreateByName,
                    CreateDate = employee.CreateDate,
                    UpdatedBy = employee.UpdatedBy,
                    UpdatedDate = employee.UpdatedDate,
                    JobPositionName = employee.JobPositionName,
                    DepartmentName = employee.DepartmentName,
                    Address = employee.Address,
                    Keyword = employee.Keyword
                };

                return PartialView("~/Views/Shared/Page/_ViewDetailEmployee.cshtml", model);
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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] object employee)
        {
            return Created("", new { message = "Employee created" });
        }

        // PUT: api/employees/5
        //[HttpPut("api/employee/update/{id:int}")]
        //public async Task<IActionResult> Update(int id, [FromBody] object employee)
        //{
           
        //}

        // DELETE: api/employee/delete/5
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
