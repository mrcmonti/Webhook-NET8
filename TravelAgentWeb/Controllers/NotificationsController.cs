using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;

namespace TravelAgentWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly TravelAgentDbContext _context;
    public NotificationsController(TravelAgentDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> FlightChanged(FlightDetailUpdateDto request)
    {
        Console.WriteLine($"Webhook Received from: {request.Publisher}");

        var secretModel = await _context
            .WebhookSecrets
            .FirstOrDefaultAsync(x => x.Publisher == request.Publisher && x.Secret == request.Secret);

        if(secretModel == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid secret - Ignore Webhook");
            Console.ResetColor();
            return Ok();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Valid webhook!");
            Console.WriteLine($"Old Price {request.OldPrice}, New Price {request.NewPrice}");
            Console.ResetColor();
            return Ok();
        }


    }
}
