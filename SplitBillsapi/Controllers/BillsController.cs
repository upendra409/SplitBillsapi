using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SplitBillsapi.ApiContracts;
using SplitBillsapi.Contracts;
using SplitBillsapi.Helpers;
using SplitBillsapi.Validators;

namespace SplitBillsapi.Controllers
{
    //[Authorize]
    //[Produces("application/json")]
    //[Route("api/v1.0/[Controller]")]
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BillsController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly IMediator _mediator;
        private IConfiguration _configuration;
        private Microsoft.Extensions.Primitives.StringValues vSignInName;
        private ExceptionHandler _exceptionHandler;
        public BillsController(IMediator mediator,IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
            vSignInName = new Microsoft.Extensions.Primitives.StringValues();
            _exceptionHandler = new ExceptionHandler();
        }
        [HttpPost]
        //[Authorize(Policy = "CheckRole")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> CreateBill([FromBody]CreateUpdateBillRequest createBillRequest)
        {
            try
            {
                Helper helper = new Helper(_configuration);
                //var validator = new BillCreateValidator();
                //var result = validator.Validate(createBillRequest);
                //if (!result.IsValid)
                //{
                //    return BadRequest(result);
                //}
                createBillRequest.TransactionId = -1;
                if (ModelState.IsValid)
                {
                    createBillRequest = await helper.Process(createBillRequest);
                    var createBillResponse = _mediator.Send(createBillRequest).Result;
                    if(createBillResponse.TransactionId == 0)
                    {
                        return BadRequest(new { message = "Operation failed due to internal issue with an application" });
                    }
                    return Ok(createBillResponse);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch(Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
        }
        [HttpGet]
        //[Authorize(Policy = "CheckRole")]
        [Route("transactionId={TransactionId}")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public IActionResult Get(int transactionId)
        {
            try
            {
                this.Request.Headers.TryGetValue("SignInName", out vSignInName);
                GetBillRequest getBillRequest = new GetBillRequest();
                getBillRequest.SignInName = vSignInName[0];
                getBillRequest.TransactionId = transactionId;
                var getBillResponse = _mediator.Send(getBillRequest).Result;
                if (getBillResponse == null)
                {
                    return BadRequest(new { message = "No rows found for the user or the transaction" });
                }
                return Ok(getBillResponse);
            }
            catch(Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
        }
        [HttpGet]
        //[Authorize(Policy = "CheckRole")]
        [MapToApiVersion("2")]
        public IActionResult Get()//public IActionResult Get(FilteringParams filteringParams)
        {
            try
            {
                this.Request.Headers.TryGetValue("SignInName", out vSignInName);
                GetUserTransactionRequest getUserTransactionRequest = new GetUserTransactionRequest();
                getUserTransactionRequest.SignInName = vSignInName[0];
                //getUserTransactionRequest.filteringParams = new FilteringParams();
                //getUserTransactionRequest.filteringParams = filteringParams;
                var getUserTransactionResponse = _mediator.Send(getUserTransactionRequest).Result;
                if (getUserTransactionResponse == null)
                {
                    return BadRequest(new { message = "No rows found for the user" });
                }
                getUserTransactionResponse.SignInName = vSignInName[0];
                return Ok(new ApiOkResponse(getUserTransactionResponse));
            }
            catch (Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
            }
        [HttpDelete]
        //[Authorize(Policy = "CheckRole")]
        [Route("transactionId={TransactionId}")]
        [MapToApiVersion("2")]
        public IActionResult Delete(int transactionId)
        {
            try
            {
                this.Request.Headers.TryGetValue("SignInName", out vSignInName);
                DeleteTransactionRequest deleteTransactionRequest = new DeleteTransactionRequest();
                deleteTransactionRequest.TransactionId = transactionId;
                deleteTransactionRequest.SignInName = vSignInName[0];
                var deleteTransactionResponse = _mediator.Send(deleteTransactionRequest).Result;
                if(deleteTransactionResponse == null)
                {
                    deleteTransactionResponse = new DeleteTransactionRequest();
                    deleteTransactionResponse.SignInName = vSignInName[0];
                    deleteTransactionResponse.TransactionId = transactionId;
                    deleteTransactionResponse.Response = "Not authorized or transaction not found";
                    return BadRequest(deleteTransactionResponse);
                }
                return Ok(deleteTransactionResponse);
            }
            catch (Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
        }
        [HttpPut]
        //[Authorize(Policy = "CheckRole")]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public async Task<IActionResult> EditBill([FromBody]CreateUpdateBillRequest createUpdateBillRequest)
        {
            try
            {
                Helper helper = new Helper(_configuration);
                //var validator = new BillCreateValidator();
                //var result = validator.Validate(createBillRequest);
                //if (!result.IsValid)
                //{
                //    return BadRequest(result);
                //}
                if (ModelState.IsValid)
                {
                    this.Request.Headers.TryGetValue("SignInName", out vSignInName);
                    createUpdateBillRequest.SignInName = vSignInName[0];
                    createUpdateBillRequest = await helper.Process(createUpdateBillRequest);
                    var createBillResponse = _mediator.Send(createUpdateBillRequest).Result;
                    return Ok(createBillResponse);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
        }

        //[HttpGet(Name = "GetBills")]
        //[MapToApiVersion("2")]
        //public IActionResult Get(FilteringParams filteringParams)
        //{
        //    return Ok();
        //}
    }
}