using DBContext.EmployeeMangement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Service.EmployeeMangement.Executes
{
    public class EmployeeManyModel
    {

        public class EmployeeResponse 
        {
            public int Id { get; set; }

            public string? Keyword { get; set; }

            public int? Status { get; set; }

            public int? CreateBy { get; set; }

            public DateTime? CreateDate { get; set; }

            public int? UpdatedBy { get; set; }

            public DateTime? UpdatedDate { get; set; }

            public string? Fullname { get; set; }

            public string? Email { get; set; }

            public string? Phone { get; set; }

            public string? Position { get; set; }
            public string? Address { get; set; }

            public string? DepartmentCode { get; set; }

            public string? DepartmentName { get; set; }

            public string? JobPositionCode { get; set; }

            public string? JobPositionName { get; set; }





        }

        public class FilterListRequest
        {
            public int Page { get; set; } = 1;
            public int? DepartmentId { get; set; }
            public int? JobpositionId { get; set; }

            public string? Position { get; set; }
            public string? KeySearch { get; set; }
            public DateTime? CreateDateFrom { get; set; }
            public DateTime? CreateDateTo { get; set; }


           



        }

        public class EmployeeListResponse
        {
            public List<EmployeeResponse> Items { get; set; } = new();
            public int TotalRecords { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
        }






    }
}
