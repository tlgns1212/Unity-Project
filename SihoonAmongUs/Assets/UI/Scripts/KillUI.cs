using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillUI : MonoBehaviour
{
    [SerializeField]
    private Image imposterImage;

    [SerializeField]
    private Image crewmateImage;

    [SerializeField]
    private Material material;

    public void Open(EPlayerColor imposter, EPlayerColor crewmate)
    {
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMoveable = false;

        Material inst1 = Instantiate(material);
        imposterImage.material = inst1;
        Material inst2 = Instantiate(material);
        crewmateImage.material = inst2;

        gameObject.SetActive(true);

        imposterImage.material.SetColor("_PlayerColor", PlayerColor.GetColor(imposter));
        crewmateImage.material.SetColor("_PlayerColor", PlayerColor.GetColor(crewmate));

        Invoke("Close", 3f);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMoveable = !(IngameUIManager.Instance.ReportUI.gameObject.activeSelf || IngameUIManager.Instance.MeetingUI.gameObject.activeSelf);

    }
}
