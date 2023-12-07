using Common.Senturion;
using Cronos;
using k8s.KubeConfigModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Graph;
using Microsoft.PowerBI.Api.Models;
using RestSharp;
using System.Collections.Concurrent;

namespace WE.Shared.Service.SignalrRHub;

public class FaceRecognitionHub : Hub
{
    public const string FaceCheckingGroup = "FaceCheckingGroup";
    public const string FaceCheckingResultFunction = "FaceCheckingResultFunction";
    public const string FaceCheckingTimeUpFunction = "FaceCheckingTimeUpFunction";


    private readonly VMSEventService _VMSEventService;
    public FaceRecognitionHub(VMSEventService vMSEventService)
    {
        this._VMSEventService = vMSEventService;
    }

    public static string GetResultGroupByConnectionId(string connectionId)
    {
        return FaceCheckingGroup + connectionId;
    }

    public static string GetTimeUpGroupByConnectionId(string connectionId)
    {
        return FaceCheckingGroup + connectionId;
    }

    public async Task ConnectToFaceRecognitionHub(string registrationId, string connectionId)
    {

        var eventModel = new VMSEventPullingItem()
        {
            Id = Guid.NewGuid().ToString(),
            ExpiredDate = DateTime.UtcNow.AddSeconds(100),
            IsActive = false,
            RegistrationId = registrationId,
            ConnectionId = connectionId
        };
        this._VMSEventService.TriggerEventPull(eventModel);

        //await Groups.AddToGroupAsync(Context.ConnectionId, GetResultGroupByConnectionId(connectionId));
    }

    public async Task DisconnectToFaceRecognitionHub(string registrationId, string connectionId)
    {
        
        //await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetResultGroupByConnectionId(connectionId));
        //_VMSEventService.TryRemoveEventPull();
    }

    public async Task ForceTimeUp(string registrationId, string connectionId)
    {
        //await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetResultGroupByConnectionId(connectionId));
        _VMSEventService.TryForceTimeUp();
    }
}

public static class FaceRecognitionHubExtension
{
    public static void AddFaceRecognition(this IServiceCollection services)
    {
        services.AddSingleton<VMSEventService>();
        services.AddHostedService<PullingVMSEventService>();
    }
}

public class VMSEventService
{
    public ConcurrentQueue<VMSEventPullingItem> HookEventQueue = new ConcurrentQueue<VMSEventPullingItem>();
    public ConcurrentQueue<VMSEventPullingItem> PullEventRequestQueue = new ConcurrentQueue<VMSEventPullingItem>();


    public bool TriggerEventPull(VMSEventPullingItem item)
    {
        this.PullEventRequestQueue.Enqueue(item);
        return true;
    }

    public bool TryActiveEvent()
    {
        VMSEventPullingItem result;

        if (this.PullEventRequestQueue.TryPeek(out result))
        {
            result.IsActive = true;
            return true;
        }

        return false;
    }

    public bool TryForceTimeUp()
    {
        VMSEventPullingItem result;

        if (this.PullEventRequestQueue.TryPeek(out result))
        {
            result.IsActive = true;
            result.ExpiredDate = DateTime.UtcNow.AddMinutes(-10);
            return true;
        }

        return false;
    }

    public VMSEventPullingItem TryGetEventPull()
    {
        VMSEventPullingItem result;

        if (this.PullEventRequestQueue.TryPeek(out result))
        {
            return result;
        }

        return null;
    }


    public VMSEventPullingItem TryRemoveEventPull()
    {
        VMSEventPullingItem result;

        if (this.PullEventRequestQueue.TryDequeue(out result))
        {
            return result;
        }

        return null;
    }
}


public class PullingVMSEventService : IHostedService, IDisposable
{
    private VMSEventService _pullingVMSService;
    private IHubContext<FaceRecognitionHub> _hub;
    private System.Timers.Timer? _timer;
    private readonly CronExpression _expression;
    private readonly ISenturionClient _senturionClient;

    public PullingVMSEventService(
        VMSEventService pullingVMSService,
        IHubContext<FaceRecognitionHub> hub,
        ISenturionClient senturionClient
        )
    {
        this._pullingVMSService = pullingVMSService;
        this._hub = hub;
        this._expression = CronExpression.Parse("0/2 * * * * *", CronFormat.IncludeSeconds);
        this._senturionClient = senturionClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ScheduleJob(cancellationToken);
    }

    public async Task DoWork(CancellationToken cancellationToken)
    {
        var pullingItem = _pullingVMSService.TryGetEventPull();
        //var snaps = await TakeSnapshot();

        if (pullingItem != null)
        {
            // remote pull event if time is up
            var expiredDate = pullingItem.ExpiredDate;
            var currentDate = DateTime.UtcNow;

            if (expiredDate <= currentDate)
            {
                await TurnOnOffAlertRule("false");

                var picture = await TakeSnapshot();

                var result = new FaceRecognitionResult()
                {
                    Result = false,
                    Message = "Time up!",
                    Image = picture
                };

                await _hub.Clients
                    .Client(pullingItem.ConnectionId)
                    .SendAsync(FaceRecognitionHub.FaceCheckingTimeUpFunction, result);

                _pullingVMSService.TryRemoveEventPull();

                return;
            }

            if (pullingItem.IsActive)
            {
                // pull event from VMS
                var pullResult = await PullCameraEvent();

                if (pullResult == null)
                {
                    return;
                }

                var resultFaceCheck = IsFaceValid(pullResult);

                if (resultFaceCheck != null)
                {
                    // case1: face registed event
                    var result = new FaceRecognitionResult();

                    if (resultFaceCheck.Result)
                    {
                        result.Result = true;
                        result.Message = "Face is registed!";
                        //var snapshot = await TakeSnapshot();
                        //result.Image = snapshot;
                        _pullingVMSService.TryRemoveEventPull();
                        await TurnOnOffAlertRule("false");

                        await _hub.Clients
                        .Client(pullingItem.ConnectionId)
                        .SendAsync(FaceRecognitionHub.FaceCheckingResultFunction, result);

                    }
                    //else
                    //{
                    //    var snapshot = await TakeSnapshot();
                    //    result.Result = false;
                    //    result.Message = "Face is not registed, please try it again!";
                    //    result.Image = snapshot;
                    //}

                    //await _hub.Clients
                    //    .Client(pullingItem.ConnectionId)
                    //    .SendAsync(FaceRecognitionHub.FaceCheckingResultFunction, result);
                }
            }
        }
    }

    private async Task TurnOnOffAlertRule(string status)
    {
        // disable alert rule
        var alertRule = "05ab8de5-6f86-4a0a-9734-575c8743bcc9";
        await this._senturionClient.AlertRule(alertRule, status);
    }

    protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
    {
        var next = _expression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Utc);

        if (next.HasValue)
        {
            var delay = next.Value - DateTimeOffset.Now;

            if (delay.TotalMilliseconds <= 0)
            {
                await ScheduleJob(cancellationToken);
            }
            _timer = new System.Timers.Timer(delay.TotalMilliseconds);
            _timer.Elapsed += async (_, _) =>
            {
                _timer.Dispose();
                _timer = null;

                if (!cancellationToken.IsCancellationRequested)
                {
                    await DoWork(cancellationToken);
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    await ScheduleJob(cancellationToken);
                }
            };

            _timer.Start();
        }

        await Task.CompletedTask;
    }

    private FaceRecognitionResult IsFaceValid(VMSDataResult model)
    {

//        var cameraId = "b225d61c-a0cd-10e2-7e65-513903f96df7";
        var cameraId = "8f8cca5a-1cd8-d112-bf4b-b780848ffb56";


        if (model.reply.Count() > 0)
        {
            var latest = model.reply.Where(x =>

            x.eventParams.metadata != null &&

            x.eventParams.metadata.cameraRefs != null &&

            x.eventParams.metadata.cameraRefs.Count() > 0 &&

            x.eventParams.metadata.cameraRefs.First() == cameraId &&

            !string.IsNullOrEmpty(x.eventParams.eventTimestampUsec) &&

            CheckValidTimeOfEvent(long.Parse(x.eventParams.eventTimestampUsec))

            );


            if (latest == null || latest.Count() < 1)
            {
                return null;
            }

            var latestEvent = latest.First();

            if (latestEvent.eventParams.description == "permittowork")
            {
                return new FaceRecognitionResult()
                {
                    Result = true,
                    Message = "registered face!"
                };
            }
            else if (latestEvent.eventParams.description == "nopermit")
            {
                return new FaceRecognitionResult()
                {
                    Result = false,
                    Message = "unregistered face!"
                };
            }
        }
        return null;
    }



    private bool CheckValidTimeOfEvent(long eventLong)
    {
        DateTimeOffset eventTime = DateTimeOffset.FromUnixTimeMilliseconds((eventLong) / 1000);
        DateTimeOffset currrentTime = DateTimeOffset.UtcNow.AddSeconds(-30);

        if (eventTime > currrentTime)
        {
            return true;
        }

        return false;
    }

    private async Task<VMSDataResult> PullCameraEvent()
    {
        try
        {
            var token = await GetVMSToken("digest123", "digest123");

            if (token != null)
            {
                var queryEventVms = new RestRequest("ec2/getEvents");

                queryEventVms.AddHeader("Authorization", "Bearer " + token.Token);
                queryEventVms
                //.AddParameter("cameraId", cameraId)
                .AddParameter("sortOrder", "desc")
                .AddParameter("event_type", "userDefinedEvent")
                .AddParameter("action_type", "showPopupAction");

                var apiVmsRequest = GetRestClient();

                var results = await apiVmsRequest.GetAsync<VMSDataResult>(queryEventVms);

                return results;

            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }

    private async Task<string> TakeSnapshot()
    {
        try
        {
            var token = await GetVMSToken("digest123", "digest123");

            if (token != null)
            {
                var queryEventVms = new RestRequest("ec2/cameraThumbnail");

                queryEventVms.AddHeader("Cookie", "x-runtime-guid=vms-0ac9b2fda2de7c3bf2c98961ae3ea28e-OChNXtzQQA");

                //string cameraID = "4d17cc24-6da3-7dff-a2ad-58f019090855";
                //string cameraID = "b225d61c-a0cd-10e2-7e65-513903f96df7";
                string cameraID = "8f8cca5a-1cd8-d112-bf4b-b780848ffb56";

                queryEventVms.AddHeader("Authorization", "Bearer " + token.Token);
                queryEventVms
                //.AddParameter("cameraId", cameraId)
                .AddParameter("cameraId", cameraID)
                .AddParameter("imageFormat", "png")
                .AddParameter("time", "now");

                var apiVmsRequest = GetRestClient();

                var results = apiVmsRequest.Execute(queryEventVms);

                if (results != null && results.RawBytes != null)
                {
                    var base64 = "data:image/png;base64," + Convert.ToBase64String(results.RawBytes);
                    return base64;
                }

            }
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        return string.Empty;
    }

    private FaceRecognitionResult FaceRecognitionFromEvent(VMSDataResult events)
    {
        var result = new FaceRecognitionResult()
        {
            Message = "Face Recognition Successful!",
            Result = true,
        };

        return result;
    }

    private async Task<TokenVMS> GetVMSToken(string userName, string password)
    {
        try
        {
            var apiVmsRequest = GetRestClient();

            var queryVms = new RestRequest("rest/v1/login/sessions");

            queryVms.AddJsonBody(new AuthenInfor
            {
                password = password,
                username = userName,
                setCookie = true
            });

            var token = await apiVmsRequest.PostAsync<TokenVMS>(queryVms);
            return token;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    private RestClient GetRestClient()
    {
        //init authen
        var handler = new HttpClientHandler();

        handler.ServerCertificateCustomValidationCallback +=
                        (sender, certificate, chain, errors) =>
                        {
                            return true;
                        };
        var option = new RestClientOptions();
        option.RemoteCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
        HttpClient clienthttp = new HttpClient(handler);
        //clienthttp.BaseAddress = new Uri("https://203.117.185.242:17001");
        clienthttp.BaseAddress = new Uri("https://66.96.193.173:17001");
        var apiVmsRequest = new RestClient(clienthttp, option);

        return apiVmsRequest;
    }


    public void Dispose()
    {
        throw new NotImplementedException();
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}


/// Model
public class VMSEventPullingItem
{
    public string Id { get; set; }
    public DateTime ExpiredDate { get; set; }
    public string RegistrationId { get; set; }
    public string ConnectionId { get; set; }
    public bool IsActive { get; set; } = false;
    //other information
}

public class FaceRecognitionResult
{
    public string Message { get; set; }
    public bool Result { get; set; }
    //other information
    public string RegistrtionId { get; set; }
    public string ConnectionId { get; set; }
    public string Image { get; set; }
}


public class ActionParams
{
    public string actionId { get; set; }
    public bool allUsers { get; set; }
    public string authType { get; set; }
    public int durationMs { get; set; }
    public bool forced { get; set; }
    public int fps { get; set; }
    public bool needConfirmation { get; set; }
    public bool playToClient { get; set; }
    public int recordAfter { get; set; }
    public int recordBeforeMs { get; set; }
    public string streamQuality { get; set; }
    public bool useSource { get; set; }
}

public class EventParams
{
    public string caption { get; set; }
    public string description { get; set; }
    public string eventTimestampUsec { get; set; }
    public string eventType { get; set; }
    public Metadata metadata { get; set; }
    public bool omitDbLogging { get; set; }
    public string reasonCode { get; set; }
    public string sourceServerId { get; set; }
}

public class Metadata
{
    public bool allUsers { get; set; }
    public List<string> cameraRefs { get; set; }
    public string level { get; set; }
}

public class Reply
{
    public ActionParams actionParams { get; set; }
    public string actionType { get; set; }
    public int aggregationCount { get; set; }
    public string businessRuleId { get; set; }
    public EventParams eventParams { get; set; }
    public int flags { get; set; }
}

public class VMSDataResult
{
    public string error { get; set; }
    public string errorId { get; set; }
    public string errorString { get; set; }
    public List<Reply> reply { get; set; }
}

public class MetaData
{
    public string level { get; set; }
}

public class AuthenInfor
{
    public string username { get; set; }
    public string password { get; set; }
    public bool setCookie { get; set; }
}
public class TokenVMS
{
    public int AgeS { get; set; }
    public int ExpiresInS { get; set; }
    public string Token { get; set; }
    public string Username { get; set; }
}