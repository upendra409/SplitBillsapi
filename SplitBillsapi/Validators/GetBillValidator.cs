using FluentValidation;
using SplitBillsapi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Validators
{
    public class GetBillValidator : AbstractValidator<GetBillRequest>
    {
        public GetBillValidator()
        {
            RuleFor(e => e.TransactionId).NotEmpty()
                .WithMessage("Transaction is a required field");
            RuleFor(e => e.SignInName).NotEmpty()
                .WithMessage("SignInName is a required field");
        }
    }
}
