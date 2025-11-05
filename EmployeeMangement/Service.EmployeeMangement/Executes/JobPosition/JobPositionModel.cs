using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EmployeeMangement.Executes
{
    public class JobPositionModel
    {
        public class JobPositionResponse
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public int? Status { get; set; }

        }

        public class DeleteJobPositionRequest
        {
            public int? Id { get; set; }
        }
    }
}
