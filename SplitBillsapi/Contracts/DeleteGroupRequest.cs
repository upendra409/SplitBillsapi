using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class DeleteGroupRequest : IRequest<string>
    {
        public int GroupId { get; set; }
        public string Response { get; set; }
    }
}
