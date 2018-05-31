using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class CreateUpdateGroupResponse
    {
        public int GroupId { get; set; }
        public string Message { get; set; }
    }
}
