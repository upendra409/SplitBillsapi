using FluentValidation;
using SplitBillsapi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Validators
{
    public class BillCreateValidator:AbstractValidator<CreateUpdateBillRequest>
    {
        public BillCreateValidator()
        {
            RuleFor(e => e.GroupId).NotEmpty()
                .WithMessage("Group is a required field");
            RuleFor(e => e.Amount).NotEmpty().GreaterThan(0)
                .WithMessage("Amount is a required field & should be greater than 0");
            //RuleFor(e => e.Participants.Count).GreaterThan(0)
            //    .WithMessage("Partipants should be present to process the request");
        }
    }
}
