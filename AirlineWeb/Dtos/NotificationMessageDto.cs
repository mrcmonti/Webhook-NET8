namespace AirlineWeb.Dtos;

public class NotificationMessageDto
{
    public string MessageId { get; } = Guid.NewGuid().ToString();
    public string Publisher { get; set; }
    public string WebhookType { get; set; }
    public string FlightCode { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
}
