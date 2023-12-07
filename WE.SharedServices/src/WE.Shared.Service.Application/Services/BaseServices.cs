using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Mongodb.Collections;
using WE.SharedService.Application.Dtos;
using Common.Identity.Users;

namespace WE.SharedService.Application.Services
{
    public interface IBaseServices<ViewModel,ModelDto,SearchViewModel>
    {
        Task<PagedList<ModelDto>> Seach(SearchViewModel searchModel);
        Task<List<LeftMenuDto>> GetLeftMenu(IList<Role> roles);
        Task<string> Create(ViewModel model);
        Task Update(string id, ViewModel model);
        Task<ModelDto> GetById(string id);

    }
    internal class BaseServices
    {
    }
}
