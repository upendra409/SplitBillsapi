using MediatR;
using SplitBillsapi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class GetUserTransactionRequest:IRequest<GetUserTransactionsResponse>
    {
        public string SignInName { get; set; }
        public FilteringParams filteringParams { get; set; }
    }
}
