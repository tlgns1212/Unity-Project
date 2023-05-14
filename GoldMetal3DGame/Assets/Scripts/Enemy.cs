using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D};
    public Type _enemyType;
    public int _maxHealth;
    public int _curHealth;
    public int _score;

    public GameManager _gameManager;
    public Transform _target;
    public BoxCollider _meleeArea;
    public GameObject _bullet;
    public GameObject[] _coins;

    protected bool _isChase;
    protected bool _isAttack;
    protected bool _isDead;

    protected Rigidbody _rigid;
    protected BoxCollider _boxCollider;
    protected MeshRenderer[] _meshs;
    protected NavMeshAgent _nav;
    protected Animator _anim;


    protected virtual void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _meshs = GetComponentsInChildren<MeshRenderer>();
        _nav = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();


        if(_enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        _isChase = true;
        _anim.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (_nav.enabled && _enemyType != Type.D)
        {
            _nav.SetDestination(_target.position);
            _nav.isStopped = !_isChase;
        }
            
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void Targeting()
    {
        if(!_isDead && _enemyType != Type.D)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (_enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }


            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
            if (rayHits.Length > 0 && !_isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        _isChase = false;
        _isAttack = true;
        _anim.SetBool("isAttack", true);

        switch (_enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                _meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                _meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                _rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                _meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                _rigid.velocity = Vector3.zero;
                _meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(_bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);

                break;
        }

        _isChase = true;
        _isAttack = false;
        _anim.SetBool("isAttack", false);
    }

    protected virtual void FreezeVelocity()
    {
        if (_isChase)
        {
            _rigid.velocity = Vector3.zero;
            _rigid.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            _curHealth = Mathf.Clamp(_curHealth - weapon._damage, 0, _maxHealth);
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            _curHealth = Mathf.Clamp(_curHealth - bullet.damage, 0, _maxHealth);
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        _curHealth = Mathf.Clamp(_curHealth - 50, 0, _maxHealth);
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, isGrenade: true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade = false)
    {
        foreach(MeshRenderer mesh in _meshs)
            mesh.material.color = Color.red;
        
        
        if(_curHealth > 0)
        {
            foreach (MeshRenderer mesh in _meshs)
                mesh.material.color = Color.white;
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            _rigid.AddForce(reactVec * 5, ForceMode.Impulse);
        }
        else
        {
            foreach (MeshRenderer mesh in _meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 12;

            if (!_isDead)
            {
                switch (_enemyType)
                {
                    case Type.A:
                        _gameManager._enemyCntA--;
                        break;
                    case Type.B:
                        _gameManager._enemyCntB--;
                        break;
                    case Type.C:
                        _gameManager._enemyCntC--;
                        break;
                    case Type.D:
                        _gameManager._enemyCntD--;
                        break;
                }
            }

            _isDead = true;
            _isChase = false;
            _nav.enabled = false;
            _anim.SetTrigger("doDie");

            Player player = _target.GetComponent<Player>();
            player._score += _score;
            int ranCoin = Random.Range(0, 3);
            Instantiate(_coins[ranCoin], transform.position, Quaternion.identity);

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                _rigid.freezeRotation = false;
                _rigid.AddForce(reactVec * 20, ForceMode.Impulse);
                _rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                _rigid.AddForce(reactVec * 20, ForceMode.Impulse);
            }
            yield return new WaitForSeconds(4f);
            Destroy(gameObject);
        }

    }
}
