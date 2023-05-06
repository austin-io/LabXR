using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringRotate : MonoBehaviour {
    
    [SerializeField] Spring3D _spring;
    [SerializeField] Transform _pivot, _lookTarget;
    
    // Start is called before the first frame update
    void Start() {
        _spring.StopAt(_lookTarget.position);
    }

    // Update is called once per frame
    void Update() {
        _spring.SetTarget(_lookTarget.position);
        _pivot.rotation = Quaternion.LookRotation((_spring.GetValue() - transform.position).normalized, transform.up);
    }
}
