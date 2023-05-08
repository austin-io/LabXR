using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkSkeleton : MonoBehaviour {
    
    [SerializeField] Transform _ikTarget, _ikBase;
    [SerializeField] List<IkBone> _bones;

    [System.Serializable]
    public class IkBone {
        public Transform xform;
        public float length;
        
        [HideInInspector]
        public Vector2 direction = Vector2.right, position = Vector2.zero;

        public Vector3 GetEnd(){
            return (xform.forward * length) + xform.position;
        }

        public Vector2 GetEnd2D(){
            return (direction * length) + position;
        }
    };
    
    // Start is called before the first frame update
    void Start() {
        for(int i = 0; i < _bones.Count; i++){
            _bones[i].position.x = i+1;
        }
    }

    Vector2 Position3DToLocal2D(Vector3 position){
        return new Vector2(Vector3.Dot(_ikBase.forward, position-_ikBase.position), Vector3.Dot(_ikBase.up, position-_ikBase.position));
    }

    Vector3 Local2DToPosition3D(Vector2 position){
        return _ikBase.position + (_ikBase.forward * position.x) + (_ikBase.up * position.y);
    }

    Vector3 Local2DTo3D(Vector2 position){
        return (_ikBase.forward * position.x) + (_ikBase.up * position.y);
    }

    // Update is called once per frame
    void Update() {
        Vector3 forwardPoint = _ikTarget.position - (_ikBase.up * Vector3.Dot(_ikTarget.position - _ikBase.position, _ikBase.up));
        forwardPoint = forwardPoint - _ikBase.position;
        _ikBase.rotation = Quaternion.LookRotation(forwardPoint.normalized, transform.up);
        
        // Alignment
        for(int i = _bones.Count - 1; i >= 0; i--){
            // 2D translation
            Vector2 targetPoint = Position3DToLocal2D(_ikTarget.position);
            
            if(i < _bones.Count - 1) targetPoint = _bones[i+1].position;
            
            _bones[i].direction = (targetPoint - _bones[i].position).normalized;
            _bones[i].position = targetPoint - (_bones[i].direction * _bones[i].length);
            
            //*
            _bones[i].xform.rotation = 
                Quaternion.LookRotation(
                    (Local2DToPosition3D(_bones[i].GetEnd2D()) - Local2DToPosition3D(_bones[i].position)).normalized,
                    _ikBase.up
                );
            _bones[i].xform.position = Local2DToPosition3D(_bones[i].position);
            //*/
        }
        
        // Return to base
        for(int i = 0; i < _bones.Count; i++){
            Vector2 targetPoint = Position3DToLocal2D(_ikBase.position);
            
            if(i > 0) targetPoint = _bones[i-1].GetEnd2D();

            _bones[i].direction = (_bones[i].GetEnd2D() - targetPoint).normalized;
            _bones[i].position = targetPoint;

            _bones[i].xform.rotation = 
                Quaternion.LookRotation(
                    (Local2DToPosition3D(_bones[i].GetEnd2D()) - Local2DToPosition3D(_bones[i].position)).normalized,
                    _ikBase.up
                );
            _bones[i].xform.position = Local2DToPosition3D(_bones[i].position);
        }
    }
}
