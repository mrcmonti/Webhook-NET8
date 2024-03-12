using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.MessageBus;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirlineWeb.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FlightsController : ControllerBase
{
    private readonly AirlineDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageBusClient _messageBus;

    public FlightsController(AirlineDbContext context, IMapper mapper, IMessageBusClient messageBus)
    {
        _context = context;
        _mapper = mapper;
        _messageBus = messageBus;
    }

    [HttpGet("{flightCode}", Name = "GetFlightDetailByCode")]
    public async Task<ActionResult<FlightDetailReadDto>> GetFlightDetailByCode(string flightCode)
    {
        var flightDetail = await _context.FlightDetails
            .FirstOrDefaultAsync(f => f.FlightCode == flightCode);

        if (flightDetail == null) { return NotFound(); }

        return Ok(_mapper.Map<FlightDetailReadDto>(flightDetail));
    }

    [HttpPost]
    public async Task<ActionResult<FlightDetailReadDto>> CreateFlight(FlightDetailCreateDto request)
    {
        var flight = await _context.FlightDetails
            .FirstOrDefaultAsync(f => f.FlightCode == request.FlightCode);

        if (flight == null)
        {
            var flightDetail = _mapper.Map<FlightDetail>(request);

            try
            {
                await _context.FlightDetails.AddAsync(flightDetail);
                await _context.SaveChangesAsync();

                var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetail);

                return CreatedAtRoute(nameof(GetFlightDetailByCode), new {flightCode = flightDetailReadDto.FlightCode}, flightDetailReadDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateFlightDetail(int id, FlightDetailUpdateDto request)
    {
        var flight = await _context.FlightDetails
            .FirstOrDefaultAsync(f => f.Id == id);

        if (flight == null) { return NotFound(); }

        decimal oldPrice = flight.Price;

        _mapper.Map(request, flight);

        try
        {
            _context.FlightDetails.Update(flight);
            await _context.SaveChangesAsync();

            if(oldPrice != flight.Price)
            {
                Console.WriteLine("Price Changed - Place message on the bus");

                var message = new NotificationMessageDto
                {
                    WebhookType = "PriceChange",
                    FlightCode = flight.FlightCode,
                    OldPrice = oldPrice,
                    NewPrice = flight.Price,
                    Publisher = "Airline"
                };

                try
                {
                    _messageBus.SendMessage(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not send message to the bus: {ex.Message}");
                }
            }

            return Ok(_mapper.Map<FlightDetailReadDto>(flight));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }
}
