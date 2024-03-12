using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirlineWeb.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WebhookSubscriptionController : ControllerBase
{
    private readonly AirlineDbContext _context;
    private readonly IMapper _mapper;
    public WebhookSubscriptionController(AirlineDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("{secret}", Name = "GetSubscriptionBySecret")]
    public async Task<ActionResult<WebhookSubscriptionReadDto>> GetSubscriptionBySecret(string secret)
    {
        var subscriptions = await _context.WebhookSubscriptions
            .FirstOrDefaultAsync(s => s.Secret == secret);

        if (subscriptions == null) { return NotFound(); }

        return Ok(_mapper.Map<IEnumerable<WebhookSubscriptionReadDto>>(subscriptions));
    }

    [HttpPost]
    public async Task<ActionResult<WebhookSubscriptionReadDto>> CreateSubscription(WebhookSubscriptionCreateDto request)
    {
        var subscription = await _context.WebhookSubscriptions
            .FirstOrDefaultAsync(s => s.WebhookURI == request.WebhookURI);

        if (subscription == null)
        {
            subscription = _mapper.Map<WebhookSubscription>(request);

            subscription.Secret = Guid.NewGuid().ToString();
            subscription.WebhookPublisher = "Airline";

            try
            {
                await _context.WebhookSubscriptions.AddAsync(subscription);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var subscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(subscription);

            return CreatedAtRoute(nameof(GetSubscriptionBySecret), new {secret = subscriptionReadDto.Secret}, subscriptionReadDto);
        }

        return NoContent();
    }
}
