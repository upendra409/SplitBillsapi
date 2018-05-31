using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class Participant
    {
        public string SignInName { get; set; }
        public string Currency { get; set; }
        public string TransactionType { get; set; }
        public float Rate { get; set; }
    }
}
