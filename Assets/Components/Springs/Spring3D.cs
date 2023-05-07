using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>Creates a spring object based on Vector3 values. Can be configured with frequency (the force multiplier) and damping (the velocity multiplier, aka friction) settings.</summary>
public class Spring3D : MonoBehaviour {

    [SerializeField] Vector3 _position, _target;
    [SerializeField] float _frequency, _damping;
    [SerializeField] bool _useDeltaTime = true;

    Vector3 _velocity = Vector3.zero;

    ///<summary>Update the spring values</summary>
    void Update() {
        float dt = (_useDeltaTime ? Time.deltaTime : 1);
        Vector3 force = (_frequency * (_target - _position));
        _velocity += force * dt;
        _position += _velocity * dt;
        _velocity *= _damping;
    }

    ///<summary>Stops the spring by setting position, target, and velocity to Vector3.zero.</summary>
    ///<params></params>
    ///<returns></returns>
    public void Stop() {
        StopAt(Vector3.zero);
    }

    ///<summary>Stops the spring by setting velocity to Vector3.zero. Position and target gets set to newTarget.</summary>
    ///<params name="newTarget">The new target and position to set immediately.</params>
    ///<returns></returns>
    public void StopAt(Vector3 newTarget) { 
        _target = newTarget;
        _position = _target;
        _velocity = Vector3.zero;
    }

    ///<summary>Gets the value of the current spring position.</summary>
    ///<returns>The current spring position.</returns>
    public Vector3 GetValue() {
        return _position;
    }

    ///<summary>Stops the spring by setting velocity to Vector3.zero. Position and target gets set to newTarget.</summary>
    ///<params name="newTarget">The new target to set immediately.</params>
    ///<returns></returns>
    public void SetTarget(Vector3 newTarget) { 
        _target = newTarget;
    }

    ///<summary>Sets the current position. Resets the velocity if necessary</summary>
    ///<params name="newPosition">The new position to set immediately.</params>
    ///<params name="resetVelocity">Should the current velocity be set to zero?</params>
    ///<returns></returns>
    public void SetPosition(Vector3 newPosition, bool resetVelocity = false) { 
        _position = newPosition;
        if(resetVelocity)
            _velocity = Vector3.zero;
    }

    ///<summary>Sets the current velocity</summary>
    ///<params name="newVelocity">The new velocity to set immediately.</params>
    ///<returns></returns>
    public void Shake(Vector3 newVelocity) {
        _velocity = newVelocity;
    }
}