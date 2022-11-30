using Mirror;
using RPG.Managers;
using System;
using System.Collections;
using UnityEngine;

public class LobbyNetworkManager : NetworkManager
{
    public static event Action OnPlayerConnect;
    public static event Action OnPlayerDisconnect;
    public static event Action<string> OnInitCharacter;

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<SetCharacterMessage>(OnCharacterCreate);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        var saveData = SaveManager.instance.GetSaveSystem();
        var setCharacter = saveData.GetLoadedCharacterData();

        Debug.Log($"{setCharacter.CharacterName} IS VALID....");

        StartCoroutine(LoadLocation(conn, setCharacter));
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
    }

    private void OnCharacterCreate(NetworkConnection conn, SetCharacterMessage message)
    {
        GameObject characterPrefab = Instantiate(playerPrefab);

        PlayerData player = characterPrefab.GetComponent<PlayerData>();

        player.SetPlayerColor(new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)));

        player.SetDisplayName(message.characterName);

        NetworkServer.AddPlayerForConnection(conn, characterPrefab);
    }
    private IEnumerator LoadLocation(NetworkConnection conn, CharacterDbContext characterData)
    {
        SetCharacterMessage newCharacterMessage = new SetCharacterMessage
        {
            characterName = characterData.CharacterName,
            currentLevel = characterData.CurrentLevel,
            currentProfession = characterData.currentProfessionType
        };
        yield return new WaitUntil(() => SceneLoader.instance.GetGamePlayStatus());
        if (SceneLoader.instance.GetGamePlayStatus()) OnPlayerConnect?.Invoke();

        yield return new WaitUntil(() => SceneLoader.instance.GetLocationStatus());
        if (SceneLoader.instance.GetLocationStatus())
        {
            conn.Send(newCharacterMessage);
        }
    }
}