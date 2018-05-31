using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class DeleteTransactionRequest:IRequest<DeleteTransactionRequest>
    {
        public int TransactionId { get; set; }
        public string SignInName { get; set; }
        public string Response { get; set; }
    }
}
