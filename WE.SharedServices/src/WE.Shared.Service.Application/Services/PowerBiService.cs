using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;

namespace WE.SharedService.Application.Services
{
    public class EmbeddedReportViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmbedUrl { get; set; }
        public string Token { get; set; }
    }

    public class PowerBiService
    {

        //private ITokenAcquisition tokenAcquisition { get; }
        private string urlPowerBiServiceApiRoot { get; }
        private IPowerBIAuthenticationService _powerBIAuthenticationService;

        public PowerBiService(
            IConfiguration configuration,
            //ITokenAcquisition tokenAcquisition
            IPowerBIAuthenticationService powerBIAuthenticationService
            )
        {
            this.urlPowerBiServiceApiRoot = configuration["PowerBi:ServiceRootUrl"];
            //this.tokenAcquisition = tokenAcquisition;
            this._powerBIAuthenticationService = powerBIAuthenticationService;
        }

        public const string powerbiApiDefaultScope = "https://analysis.windows.net/powerbi/api/.default";
        public static readonly string[] RequiredScopes = new string[] {
            "https://analysis.windows.net/powerbi/api/Report.Read.All"
        };
        // A method to get the Azure AD token (also known as 'access token')
        public string GetAccessToken()
        {
            //return this.tokenAcquisition.GetAccessTokenForAppAsync(powerbiApiDefaultScope).Result;
            var token = _powerBIAuthenticationService.GetPowerBIAAccessToken(new[] { powerbiApiDefaultScope });
            return token;
        }

        public PowerBIClient GetPowerBiClient()
        {
            var tokenCredentials = new TokenCredentials(GetAccessToken(), "Bearer");
            return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);
        }

        public async Task<EmbeddedReportViewModel> GetReport(Guid WorkspaceId, Guid ReportId)
        {

            PowerBIClient pbiClient = GetPowerBiClient();

            // Call the Power BI service API to get the embedding data
            var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId, ReportId);

            // Generate a read-only embed token for the report
            var datasetId = report.DatasetId;
            var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View, datasetId);
            var embedTokenResponse = await pbiClient.Reports.GenerateTokenAsync(WorkspaceId, ReportId, tokenRequest);
            var embedToken = embedTokenResponse.Token;
            //var fakeToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiIwMDAwMDAwMi0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8yZTliYzQzMi02MTI3LTQzZDItOTc1YS1kYTI1ZjFjOTZjZTMvIiwiaWF0IjoxNjY4NDE5MTEzLCJuYmYiOjE2Njg0MTkxMTMsImV4cCI6MTY2ODQyMzAxMywiYWlvIjoiRTJaZ1lBaVNGN3ZUS0tGbng4YTk2ak5mNjhFVEFBPT0iLCJhcHBpZCI6ImQyMzVmNzBlLTNkNDAtNGYxMC04OWJmLTI1YzdlYWNlZjIyZCIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzJlOWJjNDMyLTYxMjctNDNkMi05NzVhLWRhMjVmMWM5NmNlMy8iLCJvaWQiOiJjMjY1ZDkzZS1jMTQ4LTRlYTMtODQzMC0zYzI4MzM1ODFkOGYiLCJyaCI6IjAuQVhFQU1zU2JMaWRoMGtPWFd0b2w4Y2xzNHdJQUFBQUFBQUFBd0FBQUFBQUFBQUJ4QUFBLiIsInN1YiI6ImMyNjVkOTNlLWMxNDgtNGVhMy04NDMwLTNjMjgzMzU4MWQ4ZiIsInRlbmFudF9yZWdpb25fc2NvcGUiOiJBUyIsInRpZCI6IjJlOWJjNDMyLTYxMjctNDNkMi05NzVhLWRhMjVmMWM5NmNlMyIsInV0aSI6IjcxWllwdTgtbGt1c1ItTm9PM1FFQWciLCJ2ZXIiOiIxLjAifQ.V-mDAAydwD4XPkVD921xgeSyfL_Ca8x4pj_yjPRcw4VxMMprswf3gv5I7QcpOMSrYFutJMJZqRRNM93mIqUDPBr_Q0QLQsF1_y81bIpNyKLoIbQQ4pi9inM3OJ4L8s0WaJiYVKJlhxa870geG01U9_tuccPZaeawbN0QgwyXgJNRuO3HoCmiKhmBbE5H9Yu4TQGD8YjQl35QEeEoL0gzC6n0ku_paMwKAjRiWGrLi6voQLrnHx3Sq2k-5F7X-hT63m3iTSxdDydxX_oJbsUwXZbnl9mS-Q-ZLLFmwuua4rXmLgteNyjGrj64TPQw1eN5zr86TwoeQz5yD0dGi3Irlg";

            // Return the report embedded data to caller
            return new EmbeddedReportViewModel
            {
                Id = report.Id.ToString(),
                EmbedUrl = report.EmbedUrl,
                Name = report.Name,
                Token = embedToken
            };
        }
        public string GetAccessTokenYourOrganization()
        {
            //return this.tokenAcquisition.GetAccessTokenForUserAsync(RequiredScopes).Result;
            var token = _powerBIAuthenticationService.GetPowerBIAAccessToken(RequiredScopes);
            return token;
        }

        public PowerBIClient GetPowerBiClientYourOrganization()
        {
            var tokenCredentials = new TokenCredentials(GetAccessTokenYourOrganization(), "Bearer");
            return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);
        }

        public async Task<EmbeddedReportViewModel> GetReportYourOrganization(Guid WorkspaceId, Guid ReportId)
        {
            PowerBIClient pbiClient = GetPowerBiClientYourOrganization();
            // Call the Power BI Service API to get embedding data
            var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId, ReportId);

            // Return report embedding data to caller
            return new EmbeddedReportViewModel
            {
                Id = report.Id.ToString(),
                EmbedUrl = report.EmbedUrl,
                Name = report.Name,
                Token = GetAccessToken()
            };
        }

    }
}
