using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateXR.Manipulation;

public class SpringFollow : MonoBehaviour {
    
    [SerializeField] Spring3D _spring;
    [SerializeField] UxrGrabbableObject _grabbableObject;
    [SerializeField] Transform _followPoint;
    [SerializeField] LineRenderer _line;

    // Start is called before the first frame update
    void Start() {
        _spring.StopAt(_followPoint.position);
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
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, _followPoint.position);
        
        if(_grabbableObject.IsBeingGrabbed) return;
        _spring.SetTarget(_followPoint.position);
        transform.position = _spring.GetValue();
    }
}
