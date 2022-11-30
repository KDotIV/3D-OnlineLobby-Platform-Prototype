using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using Cinemachine;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText = null;
    //[SerializeField] private Renderer displayRenderer = null;

    [SyncVar(hook = nameof(HandleDisplayNameText))]
    [SerializeField]
    private string displayName;

    [SyncVar(hook = nameof(HandleDisplayColorUpdate))]
    [SerializeField]
    private Color playerColor = Color.black;

    private CinemachineVirtualCamera vcam;

    #region ServerCalls

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
    }
    
    [Server]
    public void SetDisplayName(string newName)
    {
        Debug.Log("Server Called SetDisplay....");
        displayName = newName;
    }
    [Server]
    public void SetPlayerColor(Color newColor)
    {
        playerColor = newColor;
    }
    [Command]
    public void CmdSetDisplayName(string newDisplayName)
    {
        Debug.Log("Command Called....");
        if(newDisplayName.Length < 2 || newDisplayName.Length > 20) { Debug.Log($"Failed to set name: {newDisplayName}"); return; }
        SetDisplayName(newDisplayName);
        RpcLogDisplayName(newDisplayName);
    }

    #endregion

    #region ClientCalls
    private void HandleDisplayColorUpdate(Color oldColor, Color newColor)
    {
        //displayRenderer.material.SetColor("_BaseColor", newColor);
    }
    private void HandleDisplayNameText(string oldText, string newText)
    {
        Debug.Log("Handler called....");
        displayNameText.text = newText;
    }

    public void SetCurrentName(string characterName)
    {
        CmdSetDisplayName(characterName);
        
    }
    [ClientRpc]
    private void RpcLogDisplayName(string newName)
    {
        Debug.Log($"Player has changed their name to: {newName}");
    }

    #endregion
}
