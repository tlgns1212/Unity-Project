using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : Enemy
{
    public GameObject _missile;
    public Transform _missilePortA;
    public Transform _missilePortB;

    Vector3 _lookVec;
    Vector3 _tauntVec;
    bool _isLook = true;

    protected override void Awake()
    {
        base.Awake();

        _nav.isStopped = true;
        StartCoroutine(Think());   
    }

    protected override void FreezeVelocity()
    {
        _rigid.velocity = Vector3.zero;
        _rigid.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (_isDead)
        {
            StopAllCoroutines();
            return;
        }
        if (_isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            _lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(_target.position + _lookVec);
        }
        else
            _nav.SetDestination(_tauntVec);
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int randAction = Random.Range(0, 5);
        switch (randAction)
        {
            case 0:
            case 1:
                // 미사일
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                // 돌
                StartCoroutine(RockShot());
                break;
            case 4:
                // 점프
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        _anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(_missile, _missilePortA.position, _missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA._target = _target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(_missile, _missilePortB.position, _missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB._target = _target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        _isLook = false;
        _anim.SetTrigger("doBigShot");
        Instantiate(_bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        _isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        _tauntVec = _target.position + _lookVec;

        _isLook = false;
        _nav.isStopped = false;
        _boxCollider.enabled = false;
        _anim.SetTrigger("doTaunt");
        yield return new WaitForSeconds(1.5f);
        _meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        _meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        _isLook = true;
        _nav.isStopped = true;
        _boxCollider.enabled = true;
        StartCoroutine(Think());
    }
}
