using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class GetBillRequest : IRequest<GetBillResponse>
    {
        public string SignInName { get; set; }
        public int TransactionId { get; set; }
    }
}
