using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyCharacterMover : CharacterMover
{
    [SyncVar(hook = nameof(SetOwnerNetId_Hook))]
    public uint ownerNetId;
    public void SetOwnerNetId_Hook(uint _, uint newOwnerId)
    {
        var players = FindObjectsOfType<AmongUsRoomPlayer>();
        foreach (var player in players)
        {
            if(newOwnerId == player.netId)
            {
                player.myCharacter = this;
                break;
            }
        }
    }

    public void CompleteSpawn()
    {
        Debug.Log("CompleteSpawn");
        if(isOwned)
        {
            IsMoveable = true;
        }
    }

}
