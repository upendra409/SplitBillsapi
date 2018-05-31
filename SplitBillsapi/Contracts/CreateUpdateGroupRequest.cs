using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class CreateUpdateGroupRequest : IRequest<int>
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<Member> listMembers { get; set; }
    }
}
