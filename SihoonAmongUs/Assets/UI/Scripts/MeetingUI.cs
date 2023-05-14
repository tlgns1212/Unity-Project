using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EMeetingState
{
    None,
    Meeting,
    Vote
}

public class MeetingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPanelPrefab;

    [SerializeField]
    private Transform playerPanelsParent;

    [SerializeField]
    private GameObject voterPrefab;

    [SerializeField]
    private GameObject skipVoteButton;

    [SerializeField]
    private GameObject skipVotePlayers;

    [SerializeField]
    private Transform skipVoteParentTransform;

    [SerializeField]
    private Text meetingTimeText;

    private EMeetingState meetingState;

    private List<MeetingPlayerPanel> meetingPlayerPanels = new List<MeetingPlayerPanel>();
    
    public void Open()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as IngameCharacterMover;
        var myPanel = Instantiate(playerPanelPrefab, playerPanelsParent).GetComponent<MeetingPlayerPanel>();
        myPanel.SetPlayer(myCharacter);
        meetingPlayerPanels.Add(myPanel);

        gameObject.SetActive(true);

        var players = FindObjectsOfType<IngameCharacterMover>();
        foreach (var player in players)
        {
            if(player != myCharacter)
            {
                var panel = Instantiate(playerPanelPrefab, playerPanelsParent).GetComponent<MeetingPlayerPanel>();
                panel.SetPlayer(player);
                meetingPlayerPanels.Add(panel);
            }
        }
    }

    public void ChangeMeetingState(EMeetingState state)
    {
        meetingState = state;
    }

    public void SelectPlayerPanel()
    {
        foreach(var panel in meetingPlayerPanels)
        {
            panel.Unselect();
        }
    }

    public void UpdateVote(EPlayerColor voterColor, EPlayerColor ejectColor)
    {
        foreach(var panel in meetingPlayerPanels)
        {
            if(panel.targetPlayer.playerColor == ejectColor)
            {
                panel.UpdatePanel(voterColor);
            }
            if(panel.targetPlayer.playerColor == voterColor)
            {
                panel.UpdateVoteSign(true);
            }
        }
    }

    public void UpdateSkipVotePlayer(EPlayerColor skipVotePlayerColor)
    {
        foreach(var panel in meetingPlayerPanels)
        {
            if(panel.targetPlayer.playerColor == skipVotePlayerColor)
            {
                panel.UpdateVoteSign(true);
            }
        }

        var voter = Instantiate(voterPrefab, skipVoteParentTransform).GetComponent<Image>();
        voter.material = Instantiate(voter.material);
        voter.material.SetColor("_PlayerColor", PlayerColor.GetColor(skipVotePlayerColor));
        skipVoteButton.SetActive(false);
    }

    public void OnClickSkipVoteButton()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as IngameCharacterMover;
        if (myCharacter.isVote)
            return;

        myCharacter.CmdSkipVote();
        SelectPlayerPanel();
    }

    public void CompleteVote()
    {
        foreach(var panel in meetingPlayerPanels)
        {
            panel.OpenResult();
        }
        skipVotePlayers.SetActive(true);
        skipVoteButton.SetActive(false);
    }

    private void Update()
    {
        if(meetingState == EMeetingState.Meeting)
        {
            meetingTimeText.text = string.Format("ȸ�ǽð� : {0}s", (int)Mathf.Clamp(GameSystem.Instance.remainTime, 0f, float.MaxValue));
        }
        else if(meetingState == EMeetingState.Vote)
        {
            meetingTimeText.text = string.Format("��ǥ�ð� : {0}s", (int)Mathf.Clamp(GameSystem.Instance.remainTime, 0f, float.MaxValue));
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
