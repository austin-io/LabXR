using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushColorChanger : MonoBehaviour {

    [SerializeField] PaintVR _painter;

    void OnTriggerEnter(Collider c){
        MeshRenderer mr = c.gameObject.GetComponent<MeshRenderer>();

        if(mr != null){
            _painter.SetColor(mr.material.color);
            GetComponent<MeshRenderer>().material.color = mr.material.color;
        }
    }

}
