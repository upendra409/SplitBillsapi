using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class GetBillResponse
    {
        public string SignInName { get; set; }
        public string GroupName { get; set; }
        public string TransactionName { get; set; }
        public decimal Amount { get; set;}
        public string Currency { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
