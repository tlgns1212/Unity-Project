using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomSettingsUI : SettingUI
{
    public void Open()
    {
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMoveable = false;
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        base.Close();
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMoveable = true;
    }

    public void ExitGameRoon()
    {
        var manager = AmongUsRoomManager.singleton;
        if(manager.mode == Mirror.NetworkManagerMode.Host)
        {
            manager.StopHost();
        }
        else if(manager.mode == Mirror.NetworkManagerMode.ClientOnly)
        {
            manager.StopClient();
        }
    }
}
