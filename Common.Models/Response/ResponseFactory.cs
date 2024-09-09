using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Response;

public static class ResponseFactory
{

    public async static Task<ApiResponse> GenerateResponse<T>(HttpResponseMessage response)
    {
        try
        {

            var json = await response.Content.ReadAsStringAsync();
            bool isValidJson = IsValidJson(json);
            JObject jsonObject = JObject.Parse(json);

            //Check if this is a default response or validation response
            if (jsonObject.GetCaseIgnoredJValue("isSuccess") != null)
            {
                return DefaultResponse<T>(jsonObject);
            }
            else
            {
                return ValidationResponse(jsonObject);
            }
        }
        catch
        {
            var apiResponse = new ApiResponse
            {
                IsSuccess = false
            };
            apiResponse.Messages.Add(Messages.NOT_200);
            apiResponse.Messages.Add(response.StatusCode.ToString());
            return apiResponse;
        }
    }

    public static string? GetCaseIgnoredJValue(this JObject jobject, string key)
    {
        return jobject.GetValue(key, StringComparison.OrdinalIgnoreCase)?.ToString();
    }

    public static ApiResponse ValidationResponse(JObject jsonObject)
    {
        ApiResponse apiResponse = new ApiResponse();
        apiResponse.IsSuccess = false;

        if (jsonObject["errors"] != null)
        {
            foreach (JToken child in jsonObject["errors"])
            {

                IJEnumerable<JToken> cemils = child.Values();
                foreach (JToken cemil in cemils)
                {
                    apiResponse.Messages.Add(cemil.ToString());
                }
            }
        }

        if (jsonObject["message"] != null)
        {
            apiResponse.Messages.Add(jsonObject["message"].Value<string>().ToString());
        }


        return apiResponse;
    }

    public static ApiResponse DefaultResponse<T>(JObject jsonObject)
    {

        ApiResponse apiResponse = new ApiResponse();
        apiResponse.IsSuccess = bool.Parse(jsonObject.GetCaseIgnoredJValue("isSuccess"));
        if (jsonObject.GetCaseIgnoredJValue("message") != null)
            apiResponse.Messages.Add((string)jsonObject.GetCaseIgnoredJValue("message"));
        // apiResponse.Messages.Add((string)jsonObject["message"]);

        if (jsonObject.GetCaseIgnoredJValue("data") != null)
        {
            var caseIgnoredData = jsonObject.GetCaseIgnoredJValue("data");
            Type type = typeof(T);
            if (type == typeof(bool))
            {
                caseIgnoredData = caseIgnoredData.ToLower();
            }

            apiResponse.Content = JsonConvert.DeserializeObject<T>(caseIgnoredData);
        }

        return apiResponse;
    }

    /// <summary>
    /// This will be called only for server ping
    /// </summary>
    public static ApiResponse GenerateDirectMessageResponse(string connectionMessage, bool success)
    {
        ApiResponse apiResponse = new()
        {
            IsSuccess = success
        };
        apiResponse.Messages.Add(connectionMessage);

        return apiResponse;
    }

    public static bool IsValidJson(string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput)) { return false; }
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException jex)
            {
                //Exception in parsing json
                Console.WriteLine(jex.Message);
                return false;
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}