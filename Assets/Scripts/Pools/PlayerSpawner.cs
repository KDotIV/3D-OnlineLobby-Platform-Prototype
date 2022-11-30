using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RPG.Managers;

public class PlayerSpawner : MonoBehaviour
{
    private void OnEnable()
    {
        LobbyNetworkManager.OnPlayerConnect += InitializePlayer;
    }
    private void OnDisable()
    {
        LobbyNetworkManager.OnPlayerDisconnect -= InitializePlayer;
    }
    private void InitializePlayer()
    {
        Debug.Log("Initializing Player....");
        StartCoroutine(TrySpawnPlayer());
    }

    private IEnumerator TrySpawnPlayer()
    {
        yield return new WaitUntil(() => SceneLoader.instance.GetLocationStatus());
        if(SceneLoader.instance.GetLocationStatus())
        {
            Transform spawnPoint = GameObject.FindGameObjectWithTag("PlayerPooler").transform;
        }
    }
}
