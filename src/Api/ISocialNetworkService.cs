namespace Api;

public interface ISocialNetworkService
{
    SocialNetworkType GetServiceType();
    Task<string> GetDataAsync(string username);
    
    public enum SocialNetworkType
    {
        Facebook,
        Instagram,
        Twitter
    }
}