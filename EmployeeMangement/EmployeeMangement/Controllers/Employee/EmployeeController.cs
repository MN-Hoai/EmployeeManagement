using EmployeeMangement.Controllers.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.EmployeeMangement.Executes;
using System.Security.Claims;
using static Service.EmployeeMangement.Executes.EmployeeManyModel;

namespace EmployeeMangement.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly EmployeeOne _employeeOne;
        private readonly EmployeeMany _employeeMany;
        public EmployeeController(EmployeeOne employeeOne, EmployeeMany employeeMany)
        {
            _employeeOne = employeeOne;
            _employeeMany = employeeMany;
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
        // GET: api/employees
        [HttpGet("api/employees")]
        public async Task<IActionResult> GetAll(FilterListRequest filter)
        {

            if (filter == null) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - dữ liệu rỗng" }); }

            var isValid = SqlGuard.IsSuspicious(filter);
            if (isValid) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - model không hợp lệ" }); }
            try
            {
                var result = _employeeMany.GetEmployees(filter);
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
                var result = _employeeOne.GetEmployee(id, null);
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
        [HttpGet("api/employee/email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            if (email.IsNullOrEmpty()) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - dữ liệu rỗng" }); }

            var isValid = SqlGuard.IsSuspicious(email);
            if (isValid) { return BadRequest(new { succsess = false, message = "Dữ liệu không hợp lệ - email không hợp lệ" }); }
            try
            {
                var result = _employeeOne.GetEmployee(0, email);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] object employee)
        {
            return Ok(new { message = $"Employee {id} updated" });
        }

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return NoContent();
        }

        // ✅ Lấy danh sách nhân viên theo ID phòng ban
        [HttpGet("api/employees/by-department/{departmentId:int}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
        {
            if (departmentId <= 0)
                return BadRequest(new { success = false, message = "ID phòng ban không hợp lệ" });

            try
            {
                var employees = await _employeeMany.GetEmployeesByDepartment(departmentId);

                if (employees == null || !employees.Any())
                    return Ok(new { success = true, message = "Phòng ban chưa có nhân viên", data = new List<object>() });

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách nhân viên theo phòng ban thành công",
                    data = employees.Select(e => new
                    {
                        e.Id,
                        fullname = e.Fullname,
                        email = e.Email,
                        phone = e.Phone
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

    }
}
