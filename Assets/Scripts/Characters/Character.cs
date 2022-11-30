using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Character : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<Character> ServerOnUnitSpawned;
    public static event Action<Character> ServerOnUnitDeSpawned;

    #region Server Calls

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDeSpawned?.Invoke(this);
    }
    #endregion

    #region Client
    [Client]
    public void Select()
    {
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        onDeselected?.Invoke();
    }

    #endregion
}
