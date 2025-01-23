using FastEndpoints;
using FluentValidation;

namespace Api;

public static class AnalyzeProfiles
{
    public record Request(string Facebook, string Instagram, string Twitter);

    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Facebook)
                .NotEmpty();
            
            RuleFor(x => x.Instagram)
                .NotEmpty();
            
            RuleFor(x => x.Twitter)
                .NotEmpty();
        }
    }
    
    public record Response(string Analysis);
    
    public class Endpoint(IServiceProvider serviceProvider, IAiService aiService) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("/analysis");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var services = serviceProvider.GetServices<ISocialNetworkService>();
            var tasks = services.Select(x =>
            {
                return x.GetServiceType() switch
                {
                    ISocialNetworkService.SocialNetworkType.Facebook => x.GetDataAsync(request.Facebook),
                    ISocialNetworkService.SocialNetworkType.Instagram => x.GetDataAsync(request.Instagram),
                    ISocialNetworkService.SocialNetworkType.Twitter => x.GetDataAsync(request.Twitter),
                    _ => null
                };
            });
            
            var allSocialNetworkData = await Task.WhenAll(tasks!);
            var socialNetworkData = string.Join("\n", allSocialNetworkData);
            var analysis = await aiService.AnalyzeDataAsync(socialNetworkData);
            var response = new Response(analysis);
            
            await SendOkAsync(response, ct);
        }
    }
}