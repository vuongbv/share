using WE.SharedService.Application.Constants;
using WE.Shared.Service.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Services;

public interface IDeviceStatusService
{
    public Task<List<string>> GetActiveDevice(List<string>? deviceIds);
}

public class DeviceStatusService : IDeviceStatusService
{
    private readonly IDevicesRepository _deviceRepository;
    public DeviceStatusService(IDevicesRepository _deviceRepository)
    {
        this._deviceRepository = _deviceRepository;
    }
    public async Task<List<string>> GetActiveDevice(List<string>? deviceIds)
    {
        var listDevice = (await _deviceRepository.FindAsyncWithoutDataAuthorize(x =>
                ((deviceIds == null || deviceIds.Count < 1) && (string.IsNullOrEmpty(x.Status) ||
                         x.Status == DeviceConstants.ActiveDevice)) || (
                deviceIds.Contains(
                        x.SourceId) &&
                        (string.IsNullOrEmpty(x.Status) ||
                         x.Status == DeviceConstants.ActiveDevice)))).ToList();

        return listDevice.Select(x => x.SourceId).ToList();
    }

}
