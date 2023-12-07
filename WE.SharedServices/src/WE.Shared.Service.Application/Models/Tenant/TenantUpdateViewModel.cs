using Common.MongodbMultitenant;
using System.ComponentModel.DataAnnotations;

namespace WE.SharedService.Application.Models.Tenant;

public class TenantUpdateViewModel 
{
    [Required]
    [MaxLength(50)]
    [RegularExpression("^[a-zA-Z0-9 ]*$")]
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Title { get; set; }
    public string IconFavicon { get; set; }
}

