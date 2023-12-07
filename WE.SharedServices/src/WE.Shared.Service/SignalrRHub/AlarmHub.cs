using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WE.Shared.Service.SignalrRHub
{
    public class AlarmHub: Hub<IAlarmHubClient>
    {
        public const string AlarmGroup = "AlarmGroup";
        public const string AlarmCreated = "AlarmCreated";
        public const string AlarmUpdated = "AlarmUpdated";
        public const string AlarmDeleted = "AlarmDeleted";

        public async Task ConnectGetAlarmRequest()
        {
            var tenantName = Context.GetHttpContext()?.Request.Query["tenant_name"];

            if(!tenantName.HasValue)
            {
                throw new TenantNotFoundException();
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, AlarmGroup + tenantName);
        }

        public async Task DisconnectionGetAlarmRequest()
        {
            var tenantName = Context.GetHttpContext()?.Request.Query["tenant_name"];

            if (!tenantName.HasValue)
            {
                throw new TenantNotFoundException();
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AlarmGroup + tenantName);
        }

    }
}
