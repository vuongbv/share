using AutoMapper;
using Common.Email;
using Common.Mongodb.Collections;
using Common.Senturion;
using Common.SMS;
using Common.SMS.Sender;
using FluentValidation;
using Microsoft.Graph;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.ApplicationName;
using WE.SharedService.Application.Models.AVAuthenticationFlow;
using WE.Shared.Service.Domain.Entities;
using WE.Shared.Service.Domain.Repository;
using System.Security.Cryptography.Xml;
using System.Web;
using System.IO;
using System.Drawing;
using SendGrid.Helpers.Mail;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;
using SendGrid;
using System.Net.Mail;
using Common.Senturian;

namespace WE.SharedService.Application.Services;

public interface IAVAuthenticationFlowService
{
    public Task<AVAuthenticationFlowDto> CreateAVAuthenticationFlow(CreateAVAuthenticationFlowModel model);
    //public Task<AVAuthenticationFlowDto> RegisterWorker(RegisterWorkerModel model);
    public Task<bool> UpdateAVAuthenticationFlow(string id, UpdateAVAuthenticationFlowModel model);
    public Task<bool> DeleteAVAuthenticationFlow(string id);
    public Task<List<AVAuthenticationListingDto>> GetAllAVAuthenticationFlow(SearchAVAuthenticationFlowViewModel search);
    public AVAuthenticationFlowDto GetAVAuthenticationFlow(string id);
    public Task<bool> RequestOTP(string id);
    public Task<AVAuthenticationFlowOTPResultDto> ValidateOTP(string id, string otp);

    public Task<SenturianApiResult> EnableAlertRules(string alertRuleId, string isEnable);
    public Task<bool> ApproveOrRegject(string registrationId, bool isApproved);
    public Task<bool> RegisterVisitor(List<VisitorModel> model);
    public List<Visitor> GetVisitor();
    public RequestRegisterDto GetRegisterInformation(string registrationId, string visitorId);
    public Task<bool> Login(string mail, string password);
}

public class AVAuthenticationFlowService : IAVAuthenticationFlowService
{
    private readonly IAVAuthenticationFlowRepository _avAuthenticationFlowStore;
    private readonly IVisitorRepository _visitorStore;
    private readonly IMapper _mapper;
    private readonly ISmsSender _smsSender;
    private readonly IEmailSender _emailSender;
    private readonly ISenturionClient _senturionClient;
    private readonly SendGridOptions _options;

    public AVAuthenticationFlowService(
        IAVAuthenticationFlowRepository avAuthenticationFlowStore,
        IVisitorRepository visitorStore,
        IMapper _mapper,
        ISmsSender smsSender,
        ISenturionClient senturionClient,
        IEmailSender emailSender,
        SendGridOptions options)
    {
        this._avAuthenticationFlowStore = avAuthenticationFlowStore;
        this._visitorStore = visitorStore;
        this._mapper = _mapper;
        this._smsSender = smsSender;
        this._senturionClient = senturionClient ?? throw new ArgumentNullException(nameof(senturionClient));
        this._emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        this._options = options;
    }

    public async Task<AVAuthenticationFlowDto> CreateAVAuthenticationFlow(CreateAVAuthenticationFlowModel model)
    {
        var entity = _mapper.Map<AVAuthenticationFlow>(model);

        entity.UpdatedDate = DateTime.UtcNow;

        //if(entity.Contractor != null && entity.Contractor.Id == null)
        //{
        //    entity.Contractor.Id = Guid.NewGuid().ToString();
        //}

        //if(entity.ListOfPersonnel != null && entity.ListOfPersonnel.Count > 0) 
        //{ 
        //    for(int i = 0; i < entity.ListOfPersonnel.Count; i++)
        //    {
        //        if (entity.ListOfPersonnel[i].Id == null)
        //        {
        //            entity.ListOfPersonnel[i].Id = Guid.NewGuid().ToString();
        //        }
        //    }
        //}

        var result = await _avAuthenticationFlowStore.InsertAsync(entity);

        return _mapper.Map<AVAuthenticationFlowDto>(result);
    }

    public async Task<List<AVAuthenticationListingDto>> GetAllAVAuthenticationFlow(SearchAVAuthenticationFlowViewModel search)
    {
        List<AVAuthenticationListingDto> results = new List<AVAuthenticationListingDto>();
        var listResult = _avAuthenticationFlowStore.GetAll().Where(x => x.Type == "Session").ToList();

        foreach (var item in listResult)
        {
            var contrator = _visitorStore.GetAll().ToList().Where(x => item.ListOfPersonnel.Contains(x.Id)).ToList();

            var nameOfContractor = contrator.Select(x => x.FirstName + " " +x.LastName).ToList();

            results.Add(new AVAuthenticationListingDto()
            {
                Id = item.Id,
                WorkFrom = item.WorkFrom,
                WorkTo = item.WorkTo,
                Purpose = item.Purpose,
                Status = item.Status,
                RequestLocations = item.Locations,
                NameOfContractors = nameOfContractor
            });
        }

        return await Task.FromResult(results);
    }

    public async Task<bool> UpdateAVAuthenticationFlow(string id, UpdateAVAuthenticationFlowModel model)
    {
        var entity = _avAuthenticationFlowStore.GetFirstOrDefault(x => x.Id == id);

        if (entity != null)
        {
            entity = _mapper.Map<AVAuthenticationFlow>(model);

            entity.UpdatedDate = DateTime.UtcNow;
            entity.Id = id;
            await _avAuthenticationFlowStore.UpdateAsync(entity, x => x.Id == id);
            return true;
        }

        return false;
    }
    public async Task<bool> DeleteAVAuthenticationFlow(string id)
    {
        await _avAuthenticationFlowStore.DeleteAsync(x => x.Id == id);
        return true;
    }
    public AVAuthenticationFlowDto GetAVAuthenticationFlow(string id)
    {
        var avAuthenticationFlow = _avAuthenticationFlowStore.GetFirstOrDefault(x => x.Id == id);
        var result = _mapper.Map<AVAuthenticationFlowDto>(avAuthenticationFlow);
        return result;
    }

    public async Task<bool> RequestOTP(string id)
    {
        var entity = _avAuthenticationFlowStore.GetFirstOrDefault(x => x.Id == id);

        if (entity != null)
        {
            var contrator = _visitorStore.GetAll().ToList().Where(x => x.Id == entity.Contractor).FirstOrDefault();
            if (contrator != null)
            {
                //Create OTP then send by SMS
                var OTP = string.Empty;
                Random random = new Random();
                OTP = random.Next(0, 999999).ToString("d6");

                try
                {
                    await _smsSender.SendSmsAsync(contrator.PhoneNumber, "Your Gtech OTP is: " + OTP + ". Please enter the OTP within 5 minutes.");
                }
                catch (Exception)
                {
                    return false;
                }

                entity.UpdatedDate = DateTime.UtcNow;
                entity.OTP = OTP;
                entity.OTPExpireDate = DateTime.UtcNow.AddMinutes(5);
                await _avAuthenticationFlowStore.UpdateAsync(entity, x => x.Id == id);
            }

            return true;
        }

        return false;
    }
    public async Task<AVAuthenticationFlowOTPResultDto> ValidateOTP(string id, string otp)
    {
        var entity = _avAuthenticationFlowStore.GetFirstOrDefault(x => x.Id == id);

        if (entity != null && !string.IsNullOrEmpty(entity.OTP) && entity.OTP.Equals(otp))
        {
            if (entity.OTPExpireDate >= DateTime.UtcNow)
            {
                // Get alert rule
                // how to mapping avAuthen with alertRule
                var alertRuleId = "";
                var isEnable = "true";
                var result = await _senturionClient.AlertRule(alertRuleId, isEnable);
                // Active alart rule

                return new AVAuthenticationFlowOTPResultDto
                {
                    Success = true,
                    OTPState = "valid"
                };
            }
            else
            {
                return new AVAuthenticationFlowOTPResultDto
                {
                    Success = false,
                    OTPState = "expired"
                };
            }
        }

        return new AVAuthenticationFlowOTPResultDto
        {
            Success = false,
            OTPState = "invalid"
        };
    }

    public async Task<SenturianApiResult> EnableAlertRules(string alertRuleId, string isEnable)
    {
        var result = await _senturionClient.AlertRule(alertRuleId, isEnable);

        return result;
    }

    public async Task<bool> ApproveOrRegject(string registrationId, bool isApproved)
    {
        var entity = _avAuthenticationFlowStore.GetFirstOrDefault(x => x.Id == registrationId && x.Type == "Session");

        if (entity != null)
        {
            entity.Status = isApproved ? WE.Shared.Service.Domain.Entities.Status.Approved : WE.Shared.Service.Domain.Entities.Status.Rejected;

            await _avAuthenticationFlowStore.UpdateAsync(entity, x => x.Id == registrationId);

            var listVisitor = _visitorStore.GetAll().ToList().Where(x => entity.ListOfPersonnel.Contains(x.Id)).ToList();

            if (isApproved && listVisitor != null && listVisitor.Count() > 0)
            {
                try
                {
                        foreach (var visitor in listVisitor)
                        {
                            string resigtraionLink = $"https://iot-dev.platform.WE.com/pocauth/contractor/day-of-work?registration_id={registrationId}&visitor_id={visitor.Id}";
                            //send link to email
                            var client = new SendGridClient(_options.SendGridAPIKey);
                            var msg = new SendGridMessage()
                            {
                                From = new EmailAddress(_options.Sender),
                                Subject = $"Access Request link of {visitor.FirstName + " " + visitor.LastName}",
                                HtmlContent = $"<p>Dear Contractor,</p>\r\n\r\n" +
                                $"<p>Your access request dated from {entity.WorkFrom.ToString()} to {entity.WorkTo.ToString()} have been approved.</p>\r\n\r\n" +
                                $"<p>On the actual date and time of your work, click or navigate to the link below for the instructions to enter the premises.</p>\r\n\r\n" +
                                $"<p>{resigtraionLink}</p>\r\n\r\n" +
                                $"<p> Sincerely,  " +
                                $"HDB</p>"
                            };
                            var tosList = new List<EmailAddress>();
                            tosList.Add(new EmailAddress(visitor.Email));
                            msg.AddTos(tosList);
                            //var response = await client.SendEmailAsync(msg);
                            await _emailSender.SendAsync(visitor.Email.Split(',').ToList(), msg.Subject, msg.HtmlContent);

                    }
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
            return true;
        }

        var visitorEntity = (await _visitorStore.FindAsync(x => x.Id == registrationId && x.Type == "Visitor")).FirstOrDefault();

        if (visitorEntity != null)
        {
            visitorEntity.Status = isApproved ? WE.Shared.Service.Domain.Entities.Status.Approved : WE.Shared.Service.Domain.Entities.Status.Rejected;

            if (isApproved)
            {
                try
                {
                    var client = new SendGridClient(_options.SendGridAPIKey);
                    var msg = new SendGridMessage()
                    {
                        From = new EmailAddress(_options.Sender),
                        Subject = "Account have been activated",
                        HtmlContent = $"<p>" +
                        $"Dear Contractor,</p>\r\n\r\n<p>" +
                        $"Your account have ben activated.  " +
                        $"Below are your temporary username and password.</p>\r\n\r\n" +
                        $"<p>Username: {visitorEntity.Email} Password: 12345</p>\r\n\r\n" +
                        $"<p>Click here to login to the Access Request portal for raising access requests and work approvals.</p>\r\n\r\n<p>" +
                        $"Alternatively, you may browse to this address to login. https://iot-dev.platform.WE.com/pocauth/contractor/login</p>\r\n\r\n" +
                        $"<p>" +
                        $"Sincerely, " +
                        $"HDB</p>"
                    };

                    var tosList = new List<EmailAddress>();
                    tosList.Add(new EmailAddress(visitorEntity.Email));
                    msg.AddTos(tosList);
                    //var response = await client.SendEmailAsync(msg);
                    await _emailSender.SendAsync(visitorEntity.Email.Split(',').ToList(), msg.Subject, msg.HtmlContent);


                    // add face
                    var addFace = await _senturionClient.AddFaceTarget(new CreateFaceTargetBody()
                    {
                        name = visitorEntity.FirstName + " " + visitorEntity.LastName,
                        category = "Contractor",
                        birthYear = 2000,
                        gender = "male",
                        description = visitorEntity.FirstName + visitorEntity.LastName
                    });
                    

                    if (addFace.HttpCode == SenturianHttpCode.OK && visitorEntity.Image != null)
                    {
                        var addfaceResult = (AddFaceTargetKeyResponse)addFace.Data;

                        visitorEntity.FaceTargetId = addfaceResult.faceTargetId;

                        // need to refactor when AV server is up
                        if (visitorEntity.Image != null)
                        {
                            var imageUrl = SaveImg(visitorEntity);
                            //to refactor . set image to url and send to VA system
                            var addKeyFaceResult = await _senturionClient.AddFaceTargetKey(addfaceResult.faceTargetId, imageUrl);
                            //isSuccess = true;
                            
                            if(addKeyFaceResult.HttpCode == SenturianHttpCode.OK)
                            {
                                var addFaceTargetKey = (UpdateFaceTargetResponse)addKeyFaceResult.Data;
                                visitorEntity.FaceTargetKeyId = addFaceTargetKey.faceTargetKeyId;
                            }
                        }
                    }
                    await _visitorStore.UpdateAsync(visitorEntity, x => x.Id == registrationId);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            await _visitorStore.UpdateAsync(visitorEntity, x => x.Id == registrationId);

        }
        return false;
    }

    public async Task<bool> RegisterVisitor(List<VisitorModel> modelModel)
    {
        //var isSuccess = false;
        foreach (var model in modelModel)
        {
            _visitorStore.Insert(_mapper.Map<Visitor>(model));
        }
        return true;
    }

    public string SaveImg(Visitor model)
    {
        try
        {
            // Tên tệp tin muốn lưu
            var rawData = model.Image.Split(",");
            string fileName = Guid.NewGuid().ToString() + $"{model.FirstName + model.LastName}.png";
            string filePath = Path.Combine("wwwroot", fileName);
            byte[] bytes = Convert.FromBase64String(rawData[1]);
            System.IO.File.WriteAllBytes(filePath, bytes);

            return filePath;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }


    public List<Visitor> GetVisitor()
    {
        return _visitorStore.GetAll().Where(x => x.Type != "Session").ToList();
    }

    public RequestRegisterDto GetRegisterInformation(string registrationId, string visitorId)
    {
        var requestInfor = _avAuthenticationFlowStore.GetFirstOrDefault(x => x.Id == registrationId && x.Type == "Session");

        if (requestInfor != null)
        {
            //get contrctor infor
            var workerId = requestInfor.ListOfPersonnel.Where(x => x == visitorId).FirstOrDefault();

            if(workerId != null)
            {
                var contrator = _visitorStore.GetFirstOrDefault(x => x.Id == requestInfor.Contractor && x.Type == "Visitor");

                if (contrator != null)
                {
                    var result = new RequestRegisterDto()
                    {
                        FirstName = contrator.FirstName,
                        LastName = contrator.LastName,
                        PhoneNumber = contrator.PhoneNumber,
                        Email = contrator.Email,
                        NumberOfWorker = workerId.ToString(),   
                        WorkTo = requestInfor.WorkTo,
                        WorkFrom = requestInfor.WorkFrom,
                        Company = contrator.Company,
                        Locations = requestInfor.Locations,
                    };

                    return result;
                }
            }

            
        }
        return new RequestRegisterDto();
    }

    public async Task<bool> Login(string mail, string password)
    {
        var entity = _visitorStore.GetFirstOrDefault(x => x.Email == mail);

        if (entity != null)
        {
            return true;
        }

        return false;
    }
}
