using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class IngameUIManager : MonoBehaviour
{
    public static IngameUIManager Instance;

    [SerializeField]
    private IngameIntroUI ingameIntroUI;
    public IngameIntroUI IngameIntroUI { get { return ingameIntroUI; } }

    [SerializeField]
    private KillButtonUI killButtonUI;
    public KillButtonUI KillButtonUI { get { return killButtonUI;} }

    [SerializeField]
    private KillUI killUI;
    public KillUI KillUI { get { return killUI; } }

    [SerializeField]
    private ReportButtonUI reportButtonUI;
    public ReportButtonUI ReportButtonUI { get { return reportButtonUI;} }

    [SerializeField]
    private ReportUI reportUI;
    public ReportUI ReportUI { get { return reportUI;} }

    [SerializeField]
    private MeetingUI meetingUI;
    public MeetingUI MeetingUI { get { return meetingUI;} }

    [SerializeField]
    private EjectionUI ejectionUI;
    public EjectionUI EjectionUI { get { return ejectionUI;} }

    [SerializeField]
    private FixWiringTask _FixWiringTaskUI;
    public FixWiringTask FixWiringTaskUI { get { return _FixWiringTaskUI; } }

    [SerializeField]
    private Button _UseButton;
    [SerializeField]
    private Sprite _OriginUseButtonSprite;

    private void Awake()
    {
        Instance = this;
    }

    public void SetUseButton(Sprite sprite, UnityAction action)
    {
        _UseButton.image.sprite = sprite;
        _UseButton.onClick.AddListener(action);
        _UseButton.interactable = true;
    }

    public void UnsetUseButton()
    {
        _UseButton.image.sprite = _OriginUseButtonSprite;
        _UseButton.onClick.RemoveAllListeners();
        _UseButton.interactable = false;
    }
}
