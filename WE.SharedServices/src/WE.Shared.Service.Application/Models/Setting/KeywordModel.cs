using WE.Shared.Service.Domain.Entities;

namespace WE.SharedService.Application.Models.Keyword;

public class KeywordModel
{
    public string Key { get; set; }
    public string Description { get; set; }
}
public class Keyword
{
    public List<KeywordModel> KeywordList { get; set; }
}
