using Microsoft.AspNetCore.Mvc;
using Service.EmployeeMangement.Executes;

namespace EmployeeMangement.Controllers
{
    public class JobPositionController : Controller
    {
        private readonly JobPositionMany _jobPositionMany;
        public JobPositionController(JobPositionMany jobPositionMany) { 
            _jobPositionMany = jobPositionMany;
        }
        public async Task<IActionResult> JobPositionList()
        {
            return PartialView("~/Views/Shared/Page/_JobPositionList.cshtml");
        }
        [HttpGet("api/jobposition/name")]
        public async Task<IActionResult> GetAllJobPositionName()
        {
            try
            {
                var results = await _jobPositionMany.GetAllJobPositionName();

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
