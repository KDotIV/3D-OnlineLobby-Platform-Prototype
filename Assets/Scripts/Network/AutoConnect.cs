using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] NetworkManager _networkManager;

    public static AutoConnect instance;

    private void Awake()
    {
        instance = this;
        _networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    public void JoinLocal()
    {
        //_networkManager.networkAddress = "40.71.122.59";
        _networkManager.StartClient();
    }
}
