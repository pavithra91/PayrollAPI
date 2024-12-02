using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leave.Contracts.Response
{
    public class EmployeeResponse
    {
        public int id { get; init; }
        public string userID { get; init; }
        public int companyCode { get; init; }
        public string costCenter { get; init; }
        public string epf { get; init; }
        public string empName { get; init; }
        public string role { get; init; }
        public string grade { get; init; }
        public bool isActive { get; init; }
    }

    public class EmployeesResponse
    {
        public IEnumerable<EmployeeResponse> Items { get; init; } = Enumerable.Empty<EmployeeResponse>();
    }
}
