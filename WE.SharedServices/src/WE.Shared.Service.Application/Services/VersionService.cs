using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using static StackExchange.Redis.Role;
using k8s;
using k8s.KubeConfigModels;

namespace WE.SharedService.Application.Services;

public interface IVersionService
{
    public Task<PackageVersion> GetPackageVersion(string packageName);
}
public class VersionService: IVersionService
{
    private PackageConfig _config;
    private IKubernetes _k8s;

    public VersionService(PackageConfig config, IKubernetes k8s)
    {
        this._config = config;
        _k8s = k8s;
    }

    public async Task<PackageVersion> GetPackageVersion(string packageName)
    {

        //switch (packageName)
        //{
        //    case "iot":
        //        {
        //            var podList = this._k8s.ListNamespacedPod("iot");
        //            var tag = podList.Items.First().Status.ContainerStatuses.First().ImageID.Split(":")[1].Substring(0, 6);

        //            return new PackageVersion()
        //            {
        //                Version = tag
        //            };
        //        }
        //    case "adminportal":
        //        {
        //            var podList = this._k8s.ListNamespacedPod("adminportal");
        //            var tag = podList.Items.First().Status.ContainerStatuses.First().ImageID.Split(":")[1].Substring(0, 6);

        //            return new PackageVersion()
        //            {
        //                Version = tag
        //            };
        //        }
        //    case "sharedservices":
        //        {
        //            var podList = this._k8s.ListNamespacedPod("sharedservices");
        //            var tag = podList.Items.First().Status.ContainerStatuses.First().ImageID.Split(":")[1].Substring(0, 6);

        //            return new PackageVersion()
        //            {
        //                Version = tag
        //            };
        //        }
        //    case "shareserviceportal":
        //        {
        //            var podList = this._k8s.ListNamespacedPod("shareserviceportal");
        //            var tag = podList.Items.First().Status.ContainerStatuses.First().ImageID.Split(":")[1].Substring(0, 6);

        //            return new PackageVersion()
        //            {
        //                Version = tag
        //            };
        //        }
        //    case "mqttbroker":
        //        {
        //            var podList = this._k8s.ListNamespacedPod("mqttbroker");
        //            var tag = podList.Items.First().Status.ContainerStatuses.First().ImageID.Split(":")[1].Substring(0, 6);

        //            return new PackageVersion()
        //            {
        //                Version = tag
        //            };
        //        }
            
        //    default:
        //        break;
        //}

        return new PackageVersion();
    }
}


public class TagsList
{
    public string name { get; set; }
    public List<string> tags { get; set; }
}

public class PackageVersion
{
    public string Version { get; set; }
    
}


public class PackageConfig
{
    public string CertificateAuthorityData { get; set; }
    public string ServerEndpoint { get; set; }
    public string ServerName { get; set; }
    public string ContextUser { get; set; }
    public string ClientCertificateData { get; set; }
    public string ClientKeyData { get; set; }
    public string Token { get; set; }
}

public static class VersionServiceExtension
{
    public static void AddVersionService(this IServiceCollection services)
    {
        services.AddScoped<IVersionService, VersionService>();
    }
    public static void AddPackageVersionConfig(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration config)
    {
        var setting = config.GetSetting<PackageConfig>("PackageConfig");

        services.AddSingleton<PackageConfig>(setting);

        services.AddTransient<IKubernetes>(ser =>
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(
            new K8SConfiguration()
            {
                ApiVersion = "v1",
                Clusters = new List<Cluster>()
                {
                       new Cluster()
                       {
                           ClusterEndpoint = new ClusterEndpoint()
                           {
                               CertificateAuthorityData = setting.CertificateAuthorityData,
                               Server = setting.ServerEndpoint,

                           },
                           Name = setting.ServerName
                       }
                },
                Contexts = new List<Context>()
                {
                       new Context()
                       {
                           ContextDetails = new ContextDetails()
                           {
                               Cluster = setting.ServerName,
                               User = setting.ContextUser
                           },
                           Name = setting.ServerName
                       }
                },
                CurrentContext = setting.ServerName,
                Kind = "Config",
                Users = new List<User>()
                {
                       new User()
                       {
                           Name = setting.ContextUser,
                           UserCredentials = new UserCredentials()
                           {
                               ClientCertificateData = setting.ClientCertificateData,
                               ClientKeyData = setting.ClientKeyData,
                               Token = setting.Token,
                           }
                       }
                }
            });

            var client = new Kubernetes(config);

            return new Kubernetes(config);
        });
    }
}
