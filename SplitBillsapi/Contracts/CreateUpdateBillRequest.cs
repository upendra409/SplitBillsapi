using MediatR;
using SplitBillsapi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class CreateUpdateBillRequest : IRequest<CreateUpdateBillResponse>
    {
        public int TransactionId { get; set; }
        //[Required]
        //[MinLength(3)]
        public string TransactionName { get; set; }
        //[Required]
        public int GroupId { get; set; }
        //[Required]
        public float Amount { get; set; }
        //[Required]
        public List<Participant> Participants { get; set; }
        //[Required]
        public string Currency { get; set; }
        public string SignInName { get; set; } 
    }
}
