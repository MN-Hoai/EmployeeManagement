using Microsoft.AspNetCore.Mvc;
using Service.EmployeeMangement.Executes;

namespace EmployeeMangement.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly DepartmentMany _departmentMany;
        public DepartmentController(DepartmentMany departmentMany )
        {
            _departmentMany = departmentMany;
        }
        public async Task<IActionResult> DepartmentList()
        {
            return PartialView("~/Views/Shared/Page/_DepartmentList.cshtml");
        }

        [HttpGet("api/departments/name")]
        public async Task<IActionResult> GetAllDepartmentName()
        {
            try
            {
                var results = await _departmentMany.GetAllDepartmentName();

                if (results == null || !results.Any())
                {
                    return NotFound(new { success = false, message = "Không có dữ liệu" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy dữ liệu thành công",
                    data = results
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Không thể kết nối server" });
            }
        }

    }
}
