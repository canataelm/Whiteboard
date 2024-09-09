using System.Net;

namespace Client.ApiConnection.Services;

public class ConnectivityService(HttpConfiguration http)
{

    public async Task<string> CheckConnection()
    {
		try
		{
			if (!await IsInternetConnected()) throw new Exception("You are not connected to the internet");
			if (!await IsServerAlive()) throw new Exception("Server is offline");

            return string.Empty;           
		}
		catch (Exception e)
		{
			return e.Message;
		}
    }

    private async Task<bool> IsInternetConnected()
    {
        try
        {
            using (var client = new WebClient())
            using (client.OpenRead("http://clients3.google.com/generate_204"))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }




    private async Task<bool> IsServerAlive()
    {
		try
		{
			using HttpResponseMessage response = await http.HttpClient.GetAsync(ApiConstants.ALIVE);
			return true;
		}
		catch
		{
			return false;
		}
    }




}
