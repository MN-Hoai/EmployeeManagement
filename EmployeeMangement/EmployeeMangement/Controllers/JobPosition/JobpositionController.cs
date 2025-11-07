using Microsoft.AspNetCore.Mvc;
using Service.EmployeeMangement;
using Service.EmployeeMangement.Executes;
using static Service.EmployeeMangement.Executes.DepartmentModel;
using static Service.EmployeeMangement.Executes.JobPositionModel;

namespace EmployeeMangement.Controllers
{
    public class JobPositionController : Controller
    {
        private readonly JobPositionMany _jobPositionMany;
        private readonly JobPositionCommand _jobPositionCommand;
        private readonly JobPositionOne _jobPositionOne;
        public JobPositionController(JobPositionMany jobPositionMany, JobPositionCommand jobPositionCommand, JobPositionOne jobPositionOne) { 
            _jobPositionMany = jobPositionMany;
            _jobPositionCommand = jobPositionCommand;
            _jobPositionOne = jobPositionOne;
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



        [HttpPost("api/jobposition/delete")]
        public async Task<IActionResult> DeleteJobPosition([FromBody] DeleteJobPositionRequest request)
        {
            try
            {
                var result = await _jobPositionCommand.DeleteJobPositionById(request.Id);
                if (result)
                    return Ok(new { success = true, message = "Xóa bài viết thành công" });

                return BadRequest(new { success = false, message = "Xóa bài viết không thành công" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Lỗi kết nối server" });
            }
        }

        [HttpGet("api/jobposition/view/{id}")]
        public async Task<IActionResult> GetJobPosition(int id)
        {
            var result = await _jobPositionOne.GetJobPositionById(id);

            if (result == null)
                return BadRequest(new { success = false, message = "Không tìm thấy phòng ban" });

            return Ok(new
            {
                success = true,
                message = "Lấy chi tiết phòng ban thành công",
                data = new
                {

                    id = result.Id,
                    code = result.Code,
                    name = result.Name,
                    keyword = result.Keyword,
                    status = result.Status,
                    createBy = result.CreateByNavigation == null ? null : new
                    {
                        id = result.CreateByNavigation.Id,
                        fullName = result.CreateByNavigation.Fullname
                    },
                    createDate = result.CreateDate,
                    updateBy = result.UpdateByNavigation == null ? null : new
                    {
                        id = result.UpdateByNavigation.Id,
                        fullName = result.UpdateByNavigation.Fullname
                    },
                    updatedDate = result.UpdatedDate,
                    manager = result.Manager == null ? null : new
                    {
                        id = result.Manager.Id,
                        fullname = result.Manager.Fullname,
                        email = result.Manager.Email,
                        phone = result.Manager.Phone,
                        avatar = result.Manager.Media?.FilePath
                    },
                    jobPosition = result.JobPosition == null ? null : new
                    {
                        id = result.JobPosition.Id,
                        code = result.JobPosition.Code,
                        name = result.JobPosition.Name
                    }
                }
            });

        }
        /// <summary>
        /// save
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>


    }
}
