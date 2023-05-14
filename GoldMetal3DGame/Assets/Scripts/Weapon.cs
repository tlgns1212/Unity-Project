using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum AttackType { Melee, Range};
    public AttackType _attackType;

    public int _damage;
    public int _maxAmmo;
    public int _curAmmo;
    public float _rate;
    


    public BoxCollider _meleeArea;
    public TrailRenderer _trailEffect;

    public Transform _bulletPos;
    public GameObject _bullet;
    public Transform _bulletCasePos;
    public GameObject _bulletCase;

    public void Use()
    {
        if(_attackType == AttackType.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if(_attackType == AttackType.Range && _curAmmo > 0)
        {
            _curAmmo--;
            StopCoroutine("Shoot");
            StartCoroutine("Shoot");
        }
    }

    IEnumerator Swing()
    {

        yield return new WaitForSeconds(0.1f);
        _meleeArea.enabled = true;
        _trailEffect.enabled = true;

        yield return new WaitForSeconds(0.4f);
        _meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        _trailEffect.enabled = false;
    }

    IEnumerator Shoot()
    {
        GameObject instantBullet = Instantiate(_bullet, _bulletPos.position, _bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = _bulletPos.forward * 50;
        yield return null;
        GameObject instantCase= Instantiate(_bulletCase, _bulletCasePos.position, _bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = _bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }

}
