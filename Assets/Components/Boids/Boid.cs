using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {
    
    float _constSpeed = 10;
    Vector3 _velocity = Vector3.forward;
    Vector3 _acceleration = Vector3.forward;

    public void Init(Vector3 vel, float speed){
        _velocity = vel;
        _constSpeed = speed;
    }

    public Vector3 GetVelocity(){
        return _velocity;
    }

    public void AddForce(Vector3 force){
        _acceleration += force;
    }

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {
        transform.rotation = Quaternion.LookRotation(_velocity);
        transform.position += _velocity * Time.deltaTime;
        _velocity += _acceleration * Time.deltaTime;
        _velocity = Vector3.ClampMagnitude(_velocity, _constSpeed);
    }
}
