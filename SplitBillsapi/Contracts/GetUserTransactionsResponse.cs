using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class GetUserTransactionsResponse
    {
        public string SignInName { get; set; }
        public string Currency { get; set; }
        public List<UserTransactionDetails> UserTransactionDetails { get; set; }
    }
}
