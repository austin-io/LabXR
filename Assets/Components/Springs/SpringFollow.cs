using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateXR.Manipulation;

public class SpringFollow : MonoBehaviour {
    
    [SerializeField] Spring3D _spring;
    [SerializeField] UxrGrabbableObject _grabbableObject;

    Vector3 _startPosition;

    // Start is called before the first frame update
    void Start() {
        _startPosition = transform.position;
        _spring.StopAt(_startPosition);
    }

    void OnEnable(){
        _grabbableObject.Released += ReleaseSpring;
    }

    void OnDisable(){
        _grabbableObject.Released -= ReleaseSpring;
    }

    void ReleaseSpring(object sender, UxrManipulationEventArgs e){
        _spring.SetPosition(transform.position, true);
    }

    // Update is called once per frame
    void Update() {
        if(_grabbableObject.IsBeingGrabbed) return;
        transform.position = _spring.GetValue();
    }
}
