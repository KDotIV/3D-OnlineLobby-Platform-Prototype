using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class UserAPI
{
    public async Task<TResultType> Get<TResultType>(string url, string authToken)
    {
        using var www = UnityWebRequest.Get(url);

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + authToken);

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Failed: {www.error}");

        var jsonResponse = www.downloadHandler.text;

        try
        {
            var result = JsonConvert.DeserializeObject<TResultType>(jsonResponse);
            Debug.Log($"Success: {www.downloadHandler.text}");
            return result;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"{this}Could not parse response: {jsonResponse}. {ex.Message}");
            return default;
        }
    }

    public async Task<AuthorizationContext> PostUser(string url, string username, string password, string email)
    {
        UserDbContext newUser = new UserDbContext()
        {
            UserName = username,
            Email = email,
            Password = password,
        };

        string jsonUser = JsonConvert.SerializeObject(newUser);

        using var www = UnityWebRequest.Post(url, jsonUser);
        UploadHandler jsonHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonUser));
        www.uploadHandler = jsonHandler;

        www.SetRequestHeader("Content-Type", "application/json");

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        var jsonResponse = www.downloadHandler.text;

        try
        {
            var authResult = JsonConvert.DeserializeObject<AuthorizationContext>(jsonResponse);
            return authResult;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"{this}Could not parse response: {jsonResponse}. {ex.Message}");
            return default;
        }
    }
    public async Task<AuthorizationContext> PostLogin(string url, string username, string password)
    {
        UserDbContext loginUser = new UserDbContext()
        { 
            UserName = username,
            Password = password
        };

        string jsonUser = JsonConvert.SerializeObject(loginUser);

        using var www = UnityWebRequest.Post(url, jsonUser);
        UploadHandler jsonHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonUser));
        www.uploadHandler = jsonHandler;

        www.SetRequestHeader("Content-Type", "application/json");

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        var jsonResponse = www.downloadHandler.text;

        try
        {
            var authResult = JsonConvert.DeserializeObject<AuthorizationContext>(jsonResponse);
            return authResult;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"{this}Could not parse response: {jsonResponse}. {ex.Message}");
            return default;
        }
    }
}
