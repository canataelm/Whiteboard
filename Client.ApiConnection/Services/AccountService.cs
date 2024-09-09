using Common.Models.Dtos;
using Common.Models.Response;
using System.Text;
using System.Text.Json;

namespace Client.ApiConnection.Services;

public class AccountService(HttpConfiguration http) : ConnectivityService(http)
{
    public async Task<ApiResponse> Login(LoginDto loginDto)
    {
        string connectionResult = await CheckConnection();
        if (string.IsNullOrEmpty(connectionResult))
        {
            var json = JsonSerializer.Serialize(loginDto);

            StringContent stringContent = new(json, Encoding.UTF8, ApiConstants.MEDIA_TYPE);

            using HttpResponseMessage response = await http.HttpClient.PostAsync(ApiConstants.ACCOUNT_LOGIN, stringContent);

            return await ResponseFactory.GenerateResponse<UserResultDto>(response);         
        }
        else 
        {
            return ResponseFactory.GenerateDirectMessageResponse(connectionResult, false);
        }    
    }

}
