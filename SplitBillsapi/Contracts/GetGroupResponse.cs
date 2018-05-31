using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class GetGroupResponse
    {
        public string GroupName { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Member> ListMembers { get; set; }
    }
}
