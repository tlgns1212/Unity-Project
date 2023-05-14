using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeOrbit : MonoBehaviour
{
    public Transform _target;
    public float _orbitSpeed;
    Vector3 _offset;
    
    void Start()
    {
        _offset = transform.position - _target.position;
    }

    void Update()
    {
        transform.position = _target.position + _offset;
        transform.RotateAround(_target.position, Vector3.up, _orbitSpeed * Time.deltaTime);
        _offset = transform.position - _target.position;
    }
}
