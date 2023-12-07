using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using WE.SharedService.Application.Models.PowerBI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WE.SharedService.Application.Services;

public interface IPowerBIAuthenticationService
{
    public string GetPowerBIAAccessToken();
    public string GetPowerBIAAccessToken(string[] scope);

}

public class PowerBIAuthenticationService: IPowerBIAuthenticationService
{
    private readonly AzureAdForPowerBIOptions _azureAd;

    public PowerBIAuthenticationService(AzureAdForPowerBIOptions azureAd)
    {
        _azureAd = azureAd ?? throw new ArgumentNullException(nameof(azureAd));
    }

    public string GetPowerBIAAccessToken()
    {
        var tenantSpecificUrl = _azureAd.AuthorityUri + _azureAd.DirectoryId;

        var clientApp = ConfidentialClientApplicationBuilder
            .Create(_azureAd.ClientId)
            .WithClientSecret(_azureAd.ClientSecret)
            .WithAuthority(tenantSpecificUrl)
            .Build();

        var authenticationResult = clientApp.AcquireTokenForClient(new[] { _azureAd.Resource + "/.default" })
            .ExecuteAsync().Result;

        return authenticationResult.AccessToken;
    }

    public string GetPowerBIAAccessToken(string[] scopes)
    {
        var tenantSpecificUrl = _azureAd.AuthorityUri + _azureAd.DirectoryId;

        var clientApp = ConfidentialClientApplicationBuilder
            .Create(_azureAd.ClientId)
            .WithClientSecret(_azureAd.ClientSecret)
            .WithAuthority(tenantSpecificUrl)
            .Build();

        var authenticationResult = clientApp.AcquireTokenForClient(scopes)
            .ExecuteAsync().Result;

        return authenticationResult.AccessToken;
    }
}

