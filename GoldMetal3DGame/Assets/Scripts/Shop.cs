using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform _uiGroup;
    public Animator _anim;

    public GameObject[] _itemObj;
    public int[] _itemPrice;
    public Transform[] _itemPos;
    public TextMeshProUGUI _talkText;
    public string[] _talkData;

    Player _enterPlayer;

    public void Enter(Player player)
    {
        _enterPlayer = player;
        _uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        _anim.SetTrigger("doHello");
        _uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = _itemPrice[index];
        if (price > _enterPlayer._coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        _enterPlayer._coin -= price;
        Vector3 randVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
        Instantiate(_itemObj[index], _itemPos[index].position + randVec, _itemPos[index].rotation);
    }

    IEnumerator Talk()
    {
        _talkText.text = _talkData[1];
        yield return new WaitForSeconds(2f);
        _talkText.text = _talkData[0];
    }
}
