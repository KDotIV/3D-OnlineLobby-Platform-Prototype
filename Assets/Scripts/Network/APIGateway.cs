using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class APIGateway : MonoBehaviour
{
    //REMOVE LOCAL API STRING BEFORE PUSHING TO SERVER
    //public string BaseURL { get; private set; } = "https://arcadelobbyapi.azurewebsites.net/api/";
    public string BaseURL { get; private set; } = "http://localhost/api/";
    [SerializeField] private string authToken;
    [SerializeField] private UserDbContext _currentUser;
    [SerializeField] private CharacterDbContext _currentCharacter;
    [SerializeField] private List<CharacterDbContext> _currentCharacterList;

    public static APIGateway instance;
    public static event Action onAPISuccess;
    public static event Action onLoginFail;
    public static event Action onLoginSuccess;

    private void Awake()
    {
        instance = this;
        _currentCharacterList = new List<CharacterDbContext>();
    }

    private void OnEnable()
    {
        Registration.onRegisterRequest += RegisterUser;
        Login.onLoginRequest += LoginUser;
        CharacterCreation.onCreationRequest += CreateCharacter;
        CharacterSelect.onCharacterAPICall += GetCharacterList;
    }

    private void OnDisable()
    {
        Registration.onRegisterRequest -= RegisterUser;
        Login.onLoginRequest -= LoginUser;
        CharacterCreation.onCreationRequest -= CreateCharacter;
        CharacterSelect.onCharacterAPICall -= GetCharacterList;
    }
    private async Task<UserDbContext> GetUser(string userID)
    {
        var userUrl = "users/get/id/";
        var UserAPI = new UserAPI();

        var getResult = await UserAPI.Get<UserDbContext>($"{BaseURL}{userUrl}{userID}", authToken);

        if (getResult != null)
        {
            Debug.Log($"Useable:{getResult.UserID}/{getResult.UserName}");
            return getResult;
        }
        return null;
    }
    private async void RegisterUser(string username, string password, string email)
    {
        var userUrl = "authentication/register";
        var UserAPI = new UserAPI();

        var postResult = await UserAPI.PostUser($"{BaseURL}{userUrl}", username, password, email);

        if (postResult.Success)
        {
            Debug.Log("Registration Success.....");
            authToken = postResult.Token;
            _currentUser = postResult.User;
        }
        else
        {
            Debug.LogError("Registration failed....");
        }
    }

    private async void LoginUser(string username, string password)
    {
        var loginUrl = "authentication/login";
        var UserAPI = new UserAPI();

        var postResult = await UserAPI.PostLogin($"{BaseURL}{loginUrl}", username, password);

        if (!postResult.Success)
        {
            Debug.LogError($"Login Failed");
            onLoginFail?.Invoke();
        }

        if (postResult.Success)
        {
            Debug.Log($"Login Success");
            onLoginSuccess?.Invoke();
            authToken = postResult.Token;
            _currentUser = postResult.User;
        }
    }

    private async void CreateCharacter(UserDbContext user, string characterName)
    {
        var apiURL = "characters/createcharacter";
        var CharacterAPI = new CharacterAPI();

        var postResult = await CharacterAPI.PostCharacter($"{BaseURL}{apiURL}", authToken, user, characterName);

        if (postResult != null)
        {
            _currentCharacter = postResult;
        }
    }

    private async void GetCharacter(string characterID)
    {
        var apiURL = "characters/get/id/";
        var CharacterAPI = new CharacterAPI();

        var getResult = await CharacterAPI.Get<CharacterDbContext>($"{BaseURL}{apiURL}{characterID}", authToken);

        if (getResult != null)
        {
            getResult = _currentCharacter;
        }
    }
    private async void GetCharacterList(string userID)
    {
        var apiURL = "characters/getlist/id/";
        var CharacterAPI = new CharacterAPI();

        var getResult = await CharacterAPI.Get<List<CharacterDbContext>>($"{BaseURL}{apiURL}{userID}", authToken);

        if (getResult != null)
        {
            _currentCharacterList = getResult;
            onAPISuccess?.Invoke();
        }
    }

    public UserDbContext GetCurrentUser()
    {
        return _currentUser;
    }
    public string GetCurrentUserID()
    {
        return _currentUser.UserID;
    }
    public List<CharacterDbContext> GetCurrentCharacterList()
    {
        return _currentCharacterList;
    }
    public CharacterDbContext GetCurrentCharacter()
    {
        return _currentCharacter;
    }
    public bool IsAuthSessionActive => !string.IsNullOrEmpty(authToken);
    public void SetBaseURL(string v)
    {
        Debug.Assert(!IsAuthSessionActive);
        BaseURL = v;
    }
}
