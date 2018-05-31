using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SplitBillsapi.Contracts;
using SplitBillsapi.Helpers;

namespace SplitBillsapi.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1.0/[Controller]")]
    public class GroupsController : Controller
    {
        private readonly IMediator _mediator;
        private IConfiguration _configuration;
        private Microsoft.Extensions.Primitives.StringValues vSignInName;
        private ExceptionHandler _exceptionHandler;
        public GroupsController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
            vSignInName = new Microsoft.Extensions.Primitives.StringValues();
            _exceptionHandler = new ExceptionHandler();
        }
        [HttpPost]
        [Authorize(Policy = "CheckRole")]
        public async Task<IActionResult> CreateGroup([FromBody]CreateUpdateGroupRequest createUpdateGroupRequest)
        {
            try
            {
                Helper helper = new Helper(_configuration);
                createUpdateGroupRequest.GroupId = -1;
                CreateUpdateGroupResponse createUpdateGroupResponse = new CreateUpdateGroupResponse();
                var response = _mediator.Send(createUpdateGroupRequest).Result;
                if(response == -1)
                {
                    return BadRequest(new { message = "Operation Failure as Group with same name already exists." });
                }
                createUpdateGroupResponse.GroupId = response;
                return Ok(createUpdateGroupResponse);
            }
            catch (Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
        }
        [HttpGet]
        [Authorize(Policy = "CheckRole")]
        [Route("GroupId={GroupId}")]
        public IActionResult Get(int groupId)
        {
            try
            {
                GetGroupRequest getGroupRequest = new GetGroupRequest();
                getGroupRequest.GroupId = groupId;
                var getGroupResponse = _mediator.Send(getGroupRequest).Result;
                return Ok(getGroupResponse);
            }
            catch (Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
        }
        [HttpDelete]
        [Authorize(Policy = "CheckRole")]
        [Route("groupId={GroupId}")]
        public IActionResult Delete(int groupId)
        {
            try
            {
                DeleteGroupRequest deleteGroupRequest = new DeleteGroupRequest();
                deleteGroupRequest.GroupId = groupId;
                deleteGroupRequest.Response = _mediator.Send(deleteGroupRequest).Result;
                return Ok(deleteGroupRequest);
            }
            catch (Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
        }
        [HttpPut]
        [Authorize(Policy = "CheckRole")]
        public async Task<IActionResult> EditGroup([FromBody]CreateUpdateGroupRequest createUpdateGroupRequest)
        {
            try
            {
                Helper helper = new Helper(_configuration);
                CreateUpdateGroupResponse createUpdateGroupResponse = new CreateUpdateGroupResponse();
                var response = _mediator.Send(createUpdateGroupRequest).Result;
                if (response == 1)
                {
                    createUpdateGroupResponse.GroupId = createUpdateGroupRequest.GroupId;
                    createUpdateGroupResponse.Message = "Operation Successfull";
                    return Ok(createUpdateGroupResponse);
                }
                else
                {
                    createUpdateGroupResponse.GroupId = 0;
                    createUpdateGroupResponse.Message = "Not Authorized or Group not found";
                    return BadRequest(createUpdateGroupResponse);
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
        }
    }
}