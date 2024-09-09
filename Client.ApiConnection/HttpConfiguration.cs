namespace Client.ApiConnection;

public class HttpConfiguration
{
    private readonly HttpClient _httpClient;
    public HttpClient HttpClient => _httpClient;


    public HttpConfiguration(string apiAddress)
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(apiAddress),
            Timeout = TimeSpan.FromSeconds(30),
        };
    }

}
