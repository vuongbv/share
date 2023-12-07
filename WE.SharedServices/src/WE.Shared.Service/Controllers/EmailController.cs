using Common.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WE.Shared.Service.Controllers;


[Route("[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailSender _emailSender;

    public EmailController(IEmailSender emailSender)
    {
        this._emailSender = emailSender;
    }

    [HttpPost]
    public async Task<IActionResult> Send(string to,string title, string body)
    {
        try
        {
            await _emailSender.SendAsync(new List<string> { to }, title, body);
            return Ok();    
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    
}
