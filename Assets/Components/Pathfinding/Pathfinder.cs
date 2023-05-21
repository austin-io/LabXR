using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateXR;
using UltimateXR.Manipulation;

public class Pathfinder : MonoBehaviour {
    
    [SerializeField] int _rows = 10, _columns = 10;
    [SerializeField] float _nodeSize = 0.1f;
    [SerializeField] GameObject _anchorPrefab, _wallPrefab, _startPrefab, _targetPrefab;

    List<UxrGrabbableObject> _objects = new List<UxrGrabbableObject>();
    List<UxrGrabbableObjectAnchor> _anchors = new List<UxrGrabbableObjectAnchor>();

    UxrGrabbableObject _startNode, _endNode;
    
    bool AnchorHasObject(UxrGrabbableObjectAnchor anchor){
        return anchor.CurrentPlacedObject != null;
    }

    bool IsWall(UxrGrabbableObject _node){
        return !((_node == _startNode) || (_node == _endNode));
    }

    bool IsTarget(UxrGrabbableObject _node){
        return (_node == _endNode);
    }

    bool IsStart(UxrGrabbableObject _node){
        return (_node == _startNode);
    }

    bool IsAnchored(UxrGrabbableObject _node){
        return _node != null &&_node.CurrentAnchor != null;
    }

    UxrGrabbableObjectAnchor GetAnchor(int x, int y){
        if(y * _columns + x > _anchors.Count) return _anchors[0];

        return _anchors[y * _columns + x];
    }

    // Start is called before the first frame update
    void Start() {
        
        for(int r = 0; r < _rows; r++){
            for(int c = 0; c < _columns; c++){
                Vector3 nodePosition = transform.position + new Vector3(r * _nodeSize, 0, c * _nodeSize);
                GameObject anchorObject = Instantiate(_anchorPrefab, nodePosition, transform.rotation);
                UxrGrabbableObjectAnchor anchor = anchorObject.GetComponent<UxrGrabbableObjectAnchor>();
                _anchors.Add(anchor);

                GameObject objectToSpawn;
                if(r == 0 && c == 0) 
                    objectToSpawn = _startPrefab;
                else if(r == _rows-1 && c == _columns-1)
                    objectToSpawn = _targetPrefab;
                else if(Random.Range(0, 10) > 7) // randomly place walls
                    objectToSpawn = _wallPrefab;
                else continue;

                GameObject _go = Instantiate(objectToSpawn, nodePosition, transform.rotation);
                UxrGrabbableObject nodeObject = _go.GetComponent<UxrGrabbableObject>();
                
                if(r == 0 && c == 0)
                    _startNode = nodeObject;
                else if(r == _rows-1 && c == _columns-1)
                    _endNode = nodeObject;

                UxrGrabManager.Instance.PlaceObject(nodeObject, anchor, UxrPlacementType.Immediate, true);

            }
        }

        UxrGrabManager.Instance.ObjectPlaced += OnObjectPlaced;

    }

    void OnObjectPlaced(object sender, UxrManipulationEventArgs e) {
        // Parameter e.Grabber may be null if the object was placed procedurally
        //Debug.Log($"Object {e.GrabbableObject.name} was placed on anchor {e.GrabbableAnchor.name} by {e.Grabber.Avatar.name}");

        if(IsStart(e.GrabbableObject) || IsTarget(e.GrabbableObject)){
            Debug.Log($"Object {e.GrabbableObject.name} placed");
            if(IsAnchored(_startNode) && IsAnchored(_endNode))
                Debug.Log("Ready for Pathfinding!");
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
