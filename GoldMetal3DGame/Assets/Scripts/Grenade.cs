using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    GameObject _meshObj;
    [SerializeField]
    GameObject _effectObj;
    [SerializeField]
    Rigidbody _rigid;

    
    void Start()
    {
        StartCoroutine("Explosion");
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        _rigid.velocity = Vector3.zero;
        _rigid.angularVelocity = Vector3.zero;
        _meshObj.SetActive(false);
        _effectObj.SetActive(true);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit hit in hits)
        {
            hit.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        Destroy(gameObject, 5);
    }
}
