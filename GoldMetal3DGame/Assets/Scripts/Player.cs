using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float _speed = 15;
    [SerializeField]
    GameObject[] weapons;
    [SerializeField]
    public bool[] hasWeapons;
    [SerializeField]
    GameObject[] grenades;
    [SerializeField]
    public int _hasGrenades;
    [SerializeField]
    GameObject _grenadeObj;

    [SerializeField]
    public Camera _followCamera;
    public GameManager _gameManager;

    [SerializeField]
    GameObject _interactText;

    [SerializeField]
    public int _ammo;
    [SerializeField]
    public int _coin;
    [SerializeField]
    public int _health;
    public int _score;


    public int _maxAmmo;
    public int _maxCoin;
    public int _maxHealth;
    public int _maxhasGrenades;


    float _vAxis;
    float _hAxis;
    float _fireDelay;

    bool _walkPressed;
    bool _jumpPressed;
    bool _interactionPressed;
    bool _attackPressed;
    bool _grenadePressed;
    bool _reloadPressed;

    bool _isJump;
    bool _isDodge;
    bool _isSwap;
    bool _isFireReady = true;
    bool _isReload;
    bool _isBorder;
    bool _isDamaged;
    bool _isShop;
    bool _isDead;
    bool _swapWeapon1Pressed;
    bool _swapWeapon2Pressed;
    bool _swapWeapon3Pressed;

    Vector3 _moveVec;
    Vector3 _dodgeVec;

    Rigidbody _rigid;
    Animator _animator;
    MeshRenderer[] _meshs;

    GameObject _nearObject;
    
    public Weapon _equipWeapon;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigid = GetComponent<Rigidbody>();
        _meshs = GetComponentsInChildren<MeshRenderer>();

        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //PlayerPrefs.SetInt("MaxScore", 112032);
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        GrenadeAttack();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");
        _walkPressed = Input.GetButton("Walk");
        _jumpPressed = Input.GetButtonDown("Jump");
        _attackPressed = Input.GetButton("Fire1");
        _grenadePressed = Input.GetButtonDown("Fire2");
        _reloadPressed = Input.GetButtonDown("Reload");
        _interactionPressed = Input.GetButtonDown("Interaction");
        _swapWeapon1Pressed = Input.GetButtonDown("Swap1");
        _swapWeapon2Pressed = Input.GetButtonDown("Swap2");
        _swapWeapon3Pressed = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        _moveVec = new Vector3(_hAxis, 0, _vAxis).normalized;

        if (_isDodge)
            _moveVec = _dodgeVec;

        if (_isSwap || !_isFireReady || _isReload || _isDead)
            _moveVec = Vector3.zero;

        if(!_isBorder)
            transform.position += _moveVec * _speed * (_walkPressed ? 0.7f : 1f) * Time.deltaTime;

        _animator.SetBool("isRun", _moveVec != Vector3.zero);
        _animator.SetBool("isWalk", _walkPressed);
    }

    void Turn()
    {
        transform.LookAt(transform.position + _moveVec);

        if (_attackPressed && !_isDead)
        {
            RaycastHit hit;
            Ray ray = _followCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100))
            {
                Vector3 nextVec = hit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if(_jumpPressed && _moveVec == Vector3.zero &&  !_isJump && !_isDodge && !_isSwap && !_isDead)
        {
            _rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            _animator.SetBool("isJump", true);
            _animator.SetTrigger("doJump");
            _isJump = true;
        }
    }

    void GrenadeAttack()
    {
        if (_hasGrenades == 0)
            return;

        if(_grenadePressed && !_isReload && !_isSwap && !_isDead)
        {
            RaycastHit hit;
            Ray ray = _followCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100))
            {
                Vector3 nextVec = hit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(_grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                _hasGrenades--;
                grenades[_hasGrenades].SetActive(false);
            }
        }
    }

    void Attack()
    {
        if (_equipWeapon == null)
            return;

        _fireDelay += Time.deltaTime;
        _isFireReady = _equipWeapon._rate < _fireDelay;

        if(_attackPressed && _isFireReady && !_isDodge && !_isSwap && !_isShop && !_isDead)
        {
            _equipWeapon.Use();
            _animator.SetTrigger(_equipWeapon._attackType == Weapon.AttackType.Melee ? "doMeleeAttack" : "doRangeAttack");
            _fireDelay = 0;
        }
    }

    void Reload()
    {
        if (_equipWeapon == null)
            return;

        if (_equipWeapon._attackType == Weapon.AttackType.Melee)
            return;

        if (_ammo == 0)
            return;

        if(_reloadPressed && !_isJump && !_isDodge &&!_isSwap && _isFireReady && !_isShop && !_isDead)
        {
            _animator.SetTrigger("doReload");
            _isReload = true;
            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = Math.Min(_ammo, _equipWeapon._maxAmmo);
        _equipWeapon._curAmmo = reAmmo;
        _ammo -= reAmmo;
        _isReload = false;
    }

    void Dodge()
    {
        if (_jumpPressed && _moveVec != Vector3.zero && !_isJump && !_isDodge && !_isSwap && !_isDead)
        {
            _dodgeVec = _moveVec;
            _speed *= 2;
            _animator.SetTrigger("doDodge");
            _isDodge = true;

            transform.GetComponent<CapsuleCollider>().enabled = false;
            _rigid.isKinematic = true;
            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        _speed *= 0.5f;
        transform.GetComponent<CapsuleCollider>().enabled = true;
        _rigid.isKinematic = false;
        _isDodge = false;
    }

    void Swap()
    {


        int weaponIndex = -1;
        if (_swapWeapon1Pressed) weaponIndex = 0;
        if (_swapWeapon2Pressed) weaponIndex = 1;
        if (_swapWeapon3Pressed) weaponIndex = 2;

        if (weaponIndex == -1 ||!hasWeapons[weaponIndex] || weapons[weaponIndex].activeSelf)
            return;

        if((_swapWeapon1Pressed || _swapWeapon2Pressed || _swapWeapon3Pressed) && !_isJump && !_isDodge && !_isSwap && !_isDead)
        {
            if(_equipWeapon != null)
                _equipWeapon.gameObject.SetActive(false);
            _equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            weapons[weaponIndex].SetActive(true);

            _animator.SetTrigger("doSwap");
            _isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        _isSwap = false;
    }

    void FreezeRotation()
    {
        _rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        _isBorder = Physics.Raycast(transform.position, _moveVec, 5, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void Interaction()
    {
        if(_interactionPressed && _nearObject != null && !_isJump && !_isDodge && !_isDead)
        {
            if(_nearObject.tag == "Weapon")
            {
                Item item = _nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(_nearObject);
            }
            else if(_nearObject.tag == "Shop")
            {
                Shop shop = _nearObject.GetComponent<Shop>();
                shop.Enter(this);
                _interactText.SetActive(false);
                _isShop = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            _animator.SetBool("isJump", false);
            _isJump = false;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item._type)
            {
                case Item.Type.Ammo:
                    _ammo += item.value;
                    if (_ammo > _maxAmmo)
                        _ammo = _maxAmmo;
                    break;
                case Item.Type.Coin:
                    _coin += item.value;
                    if (_coin > _maxCoin)
                        _coin = _maxCoin;
                    break;
                case Item.Type.Heart:
                    _health += item.value;
                    if (_health > _maxHealth)
                        _health = _maxHealth;
                    break;
                case Item.Type.Grenade:
                    if(_hasGrenades < 4)
                    {
                        grenades[_hasGrenades].SetActive(true);
                        _hasGrenades += item.value;
                        if (_hasGrenades > _maxhasGrenades)
                            _hasGrenades = _maxhasGrenades;
                    }
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            if (!_isDamaged)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                _health -= enemyBullet.damage;

                bool isBossAtk = other.name == "BossMeleeArea";

                StartCoroutine(OnDamage(isBossAtk));
            }
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        _isDamaged = true;
        foreach(MeshRenderer mesh in _meshs)
        {
            mesh.material.color = Color.red;
        }

        if (isBossAtk)
            _rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        if (_health <= 0 && !_isDead)
            OnDie();

        yield return new WaitForSeconds(1f);
        foreach (MeshRenderer mesh in _meshs)
        {
            mesh.material.color = Color.white;
        }
        _isDamaged = false;
        if (isBossAtk)
            _rigid.velocity = Vector3.zero;

    }

    void OnDie()
    {
        _animator.SetTrigger("doDie");
        _isDead = true;
        _gameManager.GameOver();
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            _nearObject = other.gameObject;
            if(!_isShop)
                _interactText.SetActive(true);
        }
            


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            _nearObject = null;
        else if(other.tag == "Shop")
        {
            Shop shop = _nearObject.GetComponent<Shop>();
            shop.Exit();
            _isShop = false;
            _nearObject = null;
            _interactText.SetActive(false);
        }
    }

}
