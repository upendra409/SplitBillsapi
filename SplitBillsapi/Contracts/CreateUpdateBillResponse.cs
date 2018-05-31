using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class CreateUpdateBillResponse
    {
        public int TransactionId { get; set; }
        public string Message { get; set; }
    }
}
