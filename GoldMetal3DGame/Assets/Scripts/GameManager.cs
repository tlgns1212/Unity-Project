using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera _menuCam;
    public Camera _gameCam;

    public Player _player;
    public Boss _boss;

    public GameObject _itemShop;
    public GameObject _weaponShop;
    public GameObject _startZone;

    public int _stage;
    public float _playTime;
    public bool _isBattle;
    public int _enemyCntA;
    public int _enemyCntB;
    public int _enemyCntC;
    public int _enemyCntD;

    public Transform[] _enemyZones;
    public GameObject[] _enemies;
    public List<int> _enemyList;

    public GameObject _menuPanel;
    public GameObject _gamePanel;
    public GameObject _gameOverPanel;
    public TextMeshProUGUI _maxScoreTxt;
    public TextMeshProUGUI _scoreTxt;
    public TextMeshProUGUI _stageTxt;
    public TextMeshProUGUI _playTimeTxt;
    public TextMeshProUGUI _playerHpTxt;
    public TextMeshProUGUI _playerAmmoTxt;
    public TextMeshProUGUI _playerCoinTxt;
    public Image _weapon1Img;
    public Image _weapon2Img;
    public Image _weapon3Img;
    public Image _weaponRClickImg;
    public TextMeshProUGUI _enemyATxt;
    public TextMeshProUGUI _enemyBTxt;
    public TextMeshProUGUI _enemyCTxt;

    public TextMeshProUGUI _curScoreTxt;
    public TextMeshProUGUI _bestScoreTxt;

    public RectTransform _bossHealthGroup;
    public RectTransform _bossHealthBar;

    private void Awake()
    {
        _enemyList = new List<int>();
        _maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {
        _menuCam.gameObject.SetActive(false);
        _gameCam.gameObject.SetActive(true);

        _menuPanel.SetActive(false);
        _gamePanel.SetActive(true);

        _player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        _gamePanel.SetActive(false);
        _gameOverPanel.SetActive(true);
        _curScoreTxt.text = _scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(_player._score > maxScore)
        {
            _bestScoreTxt.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", _player._score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        _itemShop.SetActive(false);
        _weaponShop.SetActive(false);
        _startZone.SetActive(false);

        foreach (Transform zone in _enemyZones)
            zone.gameObject.SetActive(true);
        
        _isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        _player.transform.position = Vector3.up * 1f;

        _itemShop.SetActive(true);
        _weaponShop.SetActive(true);
        _startZone.SetActive(true);

        foreach (Transform zone in _enemyZones)
            zone.gameObject.SetActive(false);

        _isBattle = false;
        _stage++;
    }

    IEnumerator InBattle()
    {
        if(_stage % 5 == 0)
        {
            _enemyCntD++;
            GameObject instantEnemy = Instantiate(_enemies[3], _enemyZones[0].position, _enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy._target = _player.transform;
            enemy._gameManager = this;
            _boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int index = 0; index < _stage; index++)
            {
                int randEnemy = Random.Range(0, 3);
                _enemyList.Add(randEnemy);

                switch (randEnemy)
                {
                    case 0:
                        _enemyCntA++;
                        break;
                    case 1:
                        _enemyCntB++;
                        break;
                    case 2:
                        _enemyCntC++;
                        break;
                }
            }

            while (_enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(_enemies[_enemyList[0]], _enemyZones[ranZone].position, _enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy._target = _player.transform;
                enemy._gameManager = this;
                _enemyList.RemoveAt(0);
                yield return new WaitForSeconds(3f);
            }
        }

        while (_enemyCntA + _enemyCntB + _enemyCntC + _enemyCntD > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(4f);

        _boss = null;
        StageEnd();

    }

    private void Update()
    {
        if (_isBattle)
            _playTime += Time.deltaTime;
    }

    private void LateUpdate()
    {
        // 상단 UI
        _scoreTxt.text = string.Format("{0:n0}", _player._score);
        _stageTxt.text = "STAGE " + _stage;

        int hour = (int)(_playTime / 3600);
        int min = (int)((_playTime - hour * 3600) / 60);
        int sec = (int)(_playTime % 60);
        _playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        // 플레이어 UI
        _playerHpTxt.text = _player._health + " / " + _player._maxHealth;
        _playerCoinTxt.text = string.Format("{0:n0}", _player._coin);
        if (_player._equipWeapon == null)
            _playerAmmoTxt.text = "- / " + _player._ammo;
        else
            _playerAmmoTxt.text = _player._equipWeapon._curAmmo + " / " + _player._ammo;

        // 무기 UI
        _weapon1Img.color = new Color(1, 1, 1, _player.hasWeapons[0] ? 1 : 0);
        _weapon2Img.color = new Color(1, 1, 1, _player.hasWeapons[1] ? 1 : 0);
        _weapon3Img.color = new Color(1, 1, 1, _player.hasWeapons[2] ? 1 : 0);
        _weaponRClickImg.color = new Color(1, 1, 1, _player._hasGrenades > 0 ? 1 : 0);

        // 몬스터 숫자 UI
        _enemyATxt.text = _enemyCntA.ToString();
        _enemyBTxt.text = _enemyCntB.ToString();
        _enemyCTxt.text = _enemyCntC.ToString();

        // 보스 체력 UI
        if(_boss != null)
        {
            _bossHealthGroup.anchoredPosition = Vector3.down * 30;
            _bossHealthBar.localScale = new Vector3((float)_boss._curHealth / _boss._maxHealth, 1, 1);
        }
        else
        {
            _bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
            

    }




}
