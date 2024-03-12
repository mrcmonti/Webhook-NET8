using AutoMapper;

namespace AirlineWeb.Profiles;

public class WebhookSubscriptionProfile : Profile
{
    public WebhookSubscriptionProfile()
    {
        CreateMap<Models.WebhookSubscription, Dtos.WebhookSubscriptionReadDto>();
        CreateMap<Dtos.WebhookSubscriptionCreateDto, Models.WebhookSubscription>();
    }

}
