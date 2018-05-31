﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Contracts
{
    public class GetGroupRequest:IRequest<GetGroupResponse>
    {
        public int GroupId { get; set; }
    }
}
