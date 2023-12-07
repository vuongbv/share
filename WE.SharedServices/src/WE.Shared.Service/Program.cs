using Common.Authentication;
using Common.Authorization;
using Common.Caching;
using Common.Exceptions;
using Common.Extensions;
using Common.Mongodb.MultiTenant;
using Common.MongoDb;
using Common.RabbitMQ;
using Common.SMS;
using FluentValidation;
using MediatR;
using Microsoft.Identity.Web;
using WE.Shared.Application;
using WE.Shared.Domain.BussinesHandler;
using WE.SharedService.Application.Services;
using WE.Shared.Service.Domain.Repository;
using WE.Shared.Service.SignalrRHub;
using Common.Senturion;
using Common.Email;

// todo: need improve, add this in appsetting
var rootApi = "";

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration; 
IWebHostEnvironment environment = builder.Environment;

// Add services to the container.
builder.Services.AddProfileBase(configuration,rootApi, environment);
builder.Services.AddMongoDb(configuration);

builder.Services.AddCustomTenantMongodbStore<ITenantStoreExtended, TenantStoreExtented>();
builder.Services.AddMultiTenentConfig(configuration);
builder.Services.AddCustomMultitenantService<CustomMultiTenantService>();
builder.Services.AddCustomTenantIdentificationStratery<RequestHostTenantIdentificationStrategy>();
builder.Services.AddSmsConfig(configuration);
builder.Services.AddAutoMapper(StarhubSharedApplication.Assembly);
builder.Services.AddValidatorsFromAssembly(StarhubSharedApplication.Assembly);
builder.Services.AddSharedApplicationService();

builder.Services.AddValidatorsFromAssembly(StarhubSharedApplication.Assembly);
builder.Services.AddRedisConfig(configuration);

builder.Services.AddDynamicAuthenticationConfig(configuration);
builder.Services.AddEnrichUserInfoPolicy();

builder.Services.AddPowerBIService(configuration);

builder.Services.AddScoped(typeof(PowerBiService));

builder.Services.AddPackageVersionConfig(configuration);
builder.Services.AddVersionService();
builder.Services.AddSenturian();
builder.Services.AddSenturianSetting(configuration);
builder.Services.AddFaceRecognition();
builder.Services.AddEmailConfig(configuration);
builder.Services.AddSendGridConfig(configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://localhost:4200", "https://iot-dev.platform.WE.com")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.UseGlobalExceptionMiddleware();
app.MapHub<AlarmHub>("alarm");
app.MapHub<FaceRecognitionHub>("face-recognition");

app.Run();