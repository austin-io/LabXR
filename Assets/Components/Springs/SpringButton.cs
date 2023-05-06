using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringButton : MonoBehaviour {
    
    [SerializeField] Spring3D _scaleSpring, _positionSpring;
    [SerializeField] Vector3 _scaleTarget, _positionTarget;
    [SerializeField] List<Transform> _scaleObjects, _positionObjects;
    
    // Start is called before the first frame update
    void Start() {
        _scaleSpring.StopAt(Vector3.one);
        _positionSpring.Stop();
    }

    // Update is called once per frame
    void Update() {
        foreach(Transform obj in _scaleObjects){
            obj.localScale = _scaleSpring.GetValue();
        }

        foreach(Transform obj in _positionObjects){
            obj.localPosition = _positionSpring.GetValue();
        } 
    }

    void OnTriggerEnter(Collider c){
        _scaleSpring.SetTarget(_scaleTarget);
        _positionSpring.SetTarget(_positionTarget);
    }

    void OnTriggerExit(Collider c){
        _scaleSpring.SetTarget(Vector3.one);
        _positionSpring.SetTarget(Vector3.zero);
    }
}
