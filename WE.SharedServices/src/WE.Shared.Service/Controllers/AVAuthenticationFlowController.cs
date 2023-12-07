using Common.ControllerBase;
using Common.Email;
using Common.Mongodb.Collections;
using Common.Senturion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.ApplicationName;
using WE.SharedService.Application.Models.AVAuthenticationFlow;
using WE.SharedService.Application.Services;
using WE.Shared.Service.Domain.Entities;
using WE.Shared.Service.SignalrRHub;
using System.Linq;

namespace WE.Shared.Service.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AVAuthenticationFlowController : ApiControllerBase
    {
        private readonly IAVAuthenticationFlowService _avAuthenticationFlowService;
        private readonly VMSEventService _VMSEventService;
        private readonly IEmailSender _emailSender;

        public AVAuthenticationFlowController(
            IAVAuthenticationFlowService avAuthenticationFlowService,
            VMSEventService VMSEventService,
            IEmailSender emailSender)
        {
            this._avAuthenticationFlowService = avAuthenticationFlowService ?? throw new ArgumentNullException(nameof(avAuthenticationFlowService));
            this._VMSEventService = VMSEventService ?? throw new ArgumentNullException(nameof(_VMSEventService));
            this._emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }

        [HttpGet]
        [Produces(typeof(PagedList<AVAuthenticationListingDto>))]
        public async Task<IActionResult> GetAll([FromQuery]SearchAVAuthenticationFlowViewModel searchModel)
        {
            var result = await this._avAuthenticationFlowService.GetAllAVAuthenticationFlow(searchModel);

            var pagged = new PagedList<AVAuthenticationListingDto>(result.AsQueryable<AVAuthenticationListingDto>(), searchModel);

            return Ok(pagged); 
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAVAuthenticationFlowModel model)
        {
            var result = await _avAuthenticationFlowService.CreateAVAuthenticationFlow(model);

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateAVAuthenticationFlowModel model)
        {
            var result = await _avAuthenticationFlowService.UpdateAVAuthenticationFlow(id, model);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _avAuthenticationFlowService.DeleteAVAuthenticationFlow(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Produces(typeof(AVAuthenticationFlowDto))]
        public async Task<IActionResult> GetAVAuthenticationFlow(string id)
        {
            var result = _avAuthenticationFlowService.GetAVAuthenticationFlow(id);
            return Ok(result);
        }
        [HttpGet("register/{id}")]
        [Produces(typeof(RequestRegisterDto))]
        public async Task<IActionResult> GetRegisterInformation(string id, string visitorId)
        {
            var result = _avAuthenticationFlowService.GetRegisterInformation(id, visitorId);
            return Ok(result);
        }

        [HttpPost("otp")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> RequestOPT(string id)
        {
            var result = await _avAuthenticationFlowService.RequestOTP(id);

            return Ok(result);
        }

        [HttpPost("validateotp")]
        [Produces(typeof(AVAuthenticationFlowOTPResultDto))]
        public async Task<IActionResult> ValidateOPT(string id, string otp)
        {
            //var result =  new AVAuthenticationFlowOTPResultDto
            //{
            //    Success = true,
            //    OTPState = "valid"
            //};

            var result = await _avAuthenticationFlowService.ValidateOTP(id, otp);

            if(result != null)
            {
                var alertRule = "05ab8de5-6f86-4a0a-9734-575c8743bcc9";
                var value = "true";

                await _avAuthenticationFlowService.EnableAlertRules(alertRule, value);
            }

            return Ok(result);
        }

        [HttpGet("pull-event-hook")]
        [Produces(typeof(bool))]
        [AllowAnonymous]
        public async Task<IActionResult> HookPullEvent()
        {
            Console.WriteLine("Hook is trigged at: " + DateTime.UtcNow.ToString() + " UTC Time");

            var result = this._VMSEventService.TryActiveEvent();

            if (result)
            {
                return Ok(result);
            }

            return BadRequest("Cannot find any access request! please reload the accesss link!");
        }

        [HttpPost("approve-or-reject")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> ApproveOrReject(string id, bool isApproved)
        {
            var result = await _avAuthenticationFlowService.ApproveOrRegject(id, isApproved);
            
            return Ok(result);
        }

        //[HttpPost("register-worker")]
        //[Produces(typeof(AVAuthenticationFlowDto))]
        //public async Task<IActionResult> RegisterWorker(RegisterWorkerModel model)
        //{
        //    var result = await _avAuthenticationFlowService.RegisterWorker(model);

        //    return Ok(result);
        //}

        [HttpPost("register-visitor")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> RegisterWorker(List<VisitorModel> model)
        {
            var result = await _avAuthenticationFlowService.RegisterVisitor(model);

            return Ok(result);
        }

        [HttpGet("visitor")]
        [Produces(typeof(PagedList<Visitor>))]
        public async Task<IActionResult> GetVisitor([FromQuery]SearchVisitorModel model)
        {
            var result = _avAuthenticationFlowService.GetVisitor();

            var pagged = new PagedList<Visitor>(result.AsQueryable<Visitor>(), model);

            return Ok(pagged);
        }

        [HttpPost("login")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _avAuthenticationFlowService.Login(email, password);
            return Ok(result);  
        }


    }
}
