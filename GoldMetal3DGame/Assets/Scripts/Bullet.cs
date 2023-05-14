using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool _isMelee;
    public bool _isRock;

    private void OnCollisionEnter(Collision collision)
    {
        if(!_isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
