using AutoMapper;
using Common.Exceptions;
using Common.Identity.Authentication.AzureAdB2c;
using Common.Identity.Users;
using Common.Mongodb.Collections;
using Common.MongoDbMultitenant;
using WE.SharedService.Application.Dtos;
using WE.SharedService.Application.Models.Users;
using WE.Shared.Service.Domain.Repository;
using System.Text;

namespace WE.SharedService.Application.Services
{
    public interface IUserService
    {
        Task<PagedList<UserDto>> Search(UserSearchViewModel search);
        Task Update(string id, UserViewModel user);
        Task<bool> Delete(string id);
        Task<UserDto> GetUser(string email);

        Task<UserDto> GetUserById(string id);
        Task<UserDto> AssignTenant(string userId, string tenantId);

        Task<UserDto> CreateUser(CreateUserModel model);
        Task EndSessionUser(EndUserSessionModel model);
        Task<List<Role>> GetRoles();
    }

    public class UserService : IUserService
    {
        private readonly ITenantStoreExtended _tenantStoreExtended;
        private IUserRepository _userRepository;
        private IMapper _mapper;
        private IAzureAdB2cUserService _azureAdB2CUserService;
        private IMultiTenantService<Common.Mongodb.MultiTenant.Tenant> _multiTenantService;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ITenantStoreExtended tenantStoreExtended,
            IAzureAdB2cUserService azureAdB2CUserService,
            IMultiTenantService<Common.Mongodb.MultiTenant.Tenant> multiTenantService

            )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tenantStoreExtended = tenantStoreExtended ?? throw new ArgumentNullException(nameof(tenantStoreExtended));
            this._azureAdB2CUserService = azureAdB2CUserService ?? throw new ArgumentNullException(nameof(azureAdB2CUserService));
            this._multiTenantService = multiTenantService ?? throw new ArgumentNullException(nameof(multiTenantService));
        }

        public async Task<PagedList<UserDto>> Search(UserSearchViewModel search)
        {
            var userQuery = _userRepository.GetAll();
            var tenants = _tenantStoreExtended.GetAll();

            if (!string.IsNullOrEmpty(search.Email))
            {
                userQuery = userQuery.Where(x => x.Email.Contains(search.Email));
            }

            var result = _mapper.Map<PagedList<UserDto>>(await userQuery.ToPagedList(search));
            result.Items = result.Items.Select(i => new UserDto()
            {
                UserName = i.UserName,
                Email = i.Email,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Id = i.Id,
                TenantId = i.TenantId,
                TenantName = !string.IsNullOrEmpty(i.TenantId) ? tenants.FirstOrDefault(n => n.Id == i.TenantId)?.Name : "",
                Roles = i.Roles
            }).ToList();

            return result;
        }

        public async Task Update(string id, UserViewModel model)
        {
            var user = _userRepository.GetFirstOrDefault(i => i.Id == id);
            if (user == null) throw new Exception();

            user.Roles = model.Roles;

            await _userRepository.UpdateAsync(user, x => x.Id == id);
        }

        public async Task<UserDto> GetUser(string email)
        {
            var user = (await _userRepository.FindAsync(x => x.Email == email)).FirstOrDefault();

            if (user == null) throw new Exception($"Cannot find user: {email}");

            var currentTenant = await _multiTenantService.GetTenantAsync();

            if (currentTenant == null) throw new TenantNotFoundException();

            var result = _mapper.Map<UserDto>(user);

            result.TenantName = currentTenant.Name;
            result.TenantIconFavicon = currentTenant.IconFavicon;
            result.TenantTitle = currentTenant.Title;
            return result;
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var user = _userRepository.GetFirstOrDefault(i => i.Id == id);
            if (user == null) throw new Exception();
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> AssignTenant(string userId, string tenantId)
        {
            var user = _userRepository.GetFirstOrDefault(i => i.Id == userId);

            if (user == null) throw new Exception();

            user.TenantId = tenantId;

            await _userRepository.UpdateAsync(user, x => x.Id == userId);

            return _mapper.Map<UserDto>(user);
        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public async Task<UserDto> CreateUser(CreateUserModel userModel)
        {
            try
            {
                //find user in gtech
                var userName = RemoveSpecialCharacters(userModel.Email);

                var finedUser = (await _userRepository.FindAsync(x => x.UserName == userName)).FirstOrDefault();
                var currentTenant = await _multiTenantService.GetTenantAsync();

                if (finedUser == null && currentTenant != null)
                {
                    var userOnGtech = new Common.Identity.Users.User()
                    {
                        UserName = userName,
                        Email = userModel.Email,
                        Roles = userModel.Roles,
                        TenantId = currentTenant.Id
                    };

                    // create Strapi User
                    var createUser = await _userRepository.InsertAsync(userOnGtech);

                    if (userModel.UserSource != null && userModel.UserSource == UserSource.AzureAdB2c)
                    {
                        var createUserOnAzureModel = new UserModelB2CIdentity()
                        {
                            DisplayName = userName,
                            GivenName = userName,
                            BirthDate = DateTimeOffset.UtcNow.AddYears(-20),
                            PreferredLanguage = "de",
                            Email = userModel.Email,
                            Password = userModel.Password,
                            Surname = userName
                        };

                        var createAzureUserResult = await _azureAdB2CUserService.CreateAzureAdB2c(createUserOnAzureModel);
                    }

                    if (createUser != null)
                    {
                        return _mapper.Map<UserDto>(userOnGtech);
                    }
                }
                else if (finedUser != null && currentTenant != null)
                {
                    if (userModel.UserSource != null && userModel.UserSource == UserSource.AzureAdB2c)
                    {
                        var createUserOnAzureModel = new UserModelB2CIdentity()
                        {
                            DisplayName = userName,
                            GivenName = userName,
                            BirthDate = DateTimeOffset.UtcNow.AddYears(-20),
                            PreferredLanguage = "de",
                            Email = userModel.Email,
                            Password = userModel.Password,
                            Surname = userName
                        };

                        var createAzureUserResult = await _azureAdB2CUserService.CreateAzureAdB2c(createUserOnAzureModel);
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return null;
        }

        public async Task EndSessionUser(EndUserSessionModel model)
        {
            if (model.UserSource == UserSource.AzureAdB2c)
            {
                await _azureAdB2CUserService.EndUserSession(model.ObjectId);
            }
        }

        public async Task<bool> Delete(string id)
        {
            var gtechUser = (await _userRepository.FindAsync(x => x.Id == id)).FirstOrDefault();
            if (gtechUser != null)
            {
                await _userRepository.DeleteAsync(x => x.Id == gtechUser.Id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Role>> GetRoles()
        {
            var users = _userRepository.GetAll();
            if (users.Any())
            {
                var roles = users.ToList().SelectMany(x => x.Roles).Select(x => x.Name).Distinct();
                return roles.Where(x => !x.Equals("TenantAdmin")).ToList().Select(x => new Role { Name = x}).ToList();
            }
            return new List<Role>();
        }
    }
}
