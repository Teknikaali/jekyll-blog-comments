using Microsoft.Rest;

namespace ApplicationCore.Analytics
{
    public interface ICredentialsFactory
    {
        ServiceClientCredentials CreateApiKeyServiceCredentials(string subscriptionKey);
    }
}
