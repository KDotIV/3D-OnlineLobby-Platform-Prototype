using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitPool : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform spawnPoint;

    #region Server Call

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);

        NetworkServer.Spawn(unitInstance);

        //If you want objects to be own by the client
        //NetworkServer.Spawn(unitInstance, connectionToClient);

    }

    #endregion

    #region Client Calls

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left){return; }

        if(!hasAuthority) {return; }

        CmdSpawnUnit();
    }

    #endregion
}
