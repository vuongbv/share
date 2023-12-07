namespace WE.Shared.Service.SignalrRHub
{
    public interface IAlarmHubClient
    {
        public Task ConnectGetAlarmRequest();
        public Task DisconnectionGetAlarmRequest();
    }
}
