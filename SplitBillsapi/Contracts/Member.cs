using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class Member
    {
        public string SignInName { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}
