using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterAPI : MonoBehaviour
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
            Debug.LogError($"Failed>: {www.error}");

        var jsonResponse = www.downloadHandler.text;

        try
        {
            var result = JsonConvert.DeserializeObject<TResultType>(jsonResponse);
            return result;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"{this} Could not parse reponse: {jsonResponse}. {ex.Message}");
            return default;
        }
    }

    public async Task<CharacterDbContext> PostCharacter(string url, string authToken, UserDbContext user, string charactername)
    {
        CharacterDbContext newCharacter = new CharacterDbContext()
        {
            CharacterName = charactername,
            User = user,
        };

        string jsonCharacter = JsonConvert.SerializeObject(newCharacter);

        using var www = UnityWebRequest.Post(url, jsonCharacter);
        UploadHandler jsonHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonCharacter));
        www.uploadHandler = jsonHandler;

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + authToken);

        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        var jsonResponse = www.downloadHandler.text;

        try
        {
            var characterResult = JsonConvert.DeserializeObject<CharacterDbContext>(jsonResponse);
            return characterResult;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"{this} Could not parse response: {jsonResponse}. {ex.Message}");
            return default;
        }
    }
}
