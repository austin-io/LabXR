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
    List<Node> _nodes = new List<Node>();

    UxrGrabbableObject _startNode, _endNode;

    public class Node {
        public Node parent = null;
        public Vector2Int position;
        public bool visited = false;
        public float gCost = float.MaxValue, hCost = 1, fCost = float.MaxValue;
        public enum Type {EMPTY, START, WALL, TARGET};
        public Type type = Type.EMPTY;
    }; 
    
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
        return _node != null && _node.CurrentAnchor != null;
    }

    int GetIndex(int x, int y){
        return y * _columns + x;
    }

    UxrGrabbableObjectAnchor GetAnchor(int x, int y){
        int index = GetIndex(x,y);
        if(index > _anchors.Count || index < 0) return _anchors[0];

        return _anchors[index];
    }

    void SetAnchorColor(UxrGrabbableObjectAnchor anchor, Color color){
        anchor.gameObject.GetComponentInChildren<MeshRenderer>().material.color = color;
    }

    void ClearGrid(Color color = new Color()){
        for(int i = 0; i < _anchors.Count; i++){
            SetAnchorColor(_anchors[i], color);
        }
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
                else if(Random.Range(0, 10) > 6) // randomly place walls
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

        //SetAnchorColor(GetAnchor(0,0), Color.blue);
        StartPathfinding();

    }

    void OnObjectPlaced(object sender, UxrManipulationEventArgs e) {
        // Parameter e.Grabber may be null if the object was placed procedurally
        //Debug.Log($"Object {e.GrabbableObject.name} was placed on anchor {e.GrabbableAnchor.name} by {e.Grabber.Avatar.name}");

        if(IsStart(e.GrabbableObject) || IsTarget(e.GrabbableObject)){
            Debug.Log($"Object {e.GrabbableObject.name} placed");
            if(IsAnchored(_startNode) && IsAnchored(_endNode)){
                Debug.Log("Ready for Pathfinding!");
                StartPathfinding();
            }
        }
    }

    bool IsOutOfBounds(int x, int y){
        return x < 0 || x > _columns-1 || y < 0 || y > _rows-1;
    }

    void StartPathfinding(){
                
        ClearGrid(Color.white);
        
        List<Node> nodes = new List<Node>();
        //Vector2Int startPosition = Vector2Int.zero, targetPosition = Vector2Int.zero;
        Node startNode = null, targetNode = null;

        for(int y = 0; y < _rows; y++){
            for(int x = 0; x < _columns; x++){
                int index = GetIndex(x,y);
                Node newNode = new Node();
                newNode.position = new Vector2Int(x,y);
                
                if(!AnchorHasObject(_anchors[index])){
                    nodes.Add(newNode);
                    continue;
                }

                if(IsStart(_anchors[index].CurrentPlacedObject)){
                    //startPosition = new Vector2Int(x,y);
                    newNode.type = Node.Type.START;
                    startNode = newNode;
                } else if(IsTarget(_anchors[index].CurrentPlacedObject)){
                    //targetPosition = new Vector2Int(x,y);
                    newNode.type = Node.Type.TARGET;
                    targetNode = newNode;
                } else {
                    newNode.type = Node.Type.WALL;
                    newNode.visited = true;
                }

                nodes.Add(newNode);
            }
        }

        if(startNode == null || targetNode == null) return;
        
        List<Node> path = FindPath(nodes, startNode, targetNode);

        if(path == null){
            ClearGrid(Color.red);
            return;
        }

        for(int i = 0; i < path.Count; i++){
            int index = GetIndex(path[i].position.x, path[i].position.y);
            SetAnchorColor(_anchors[index], Color.blue);
        }
    }

    List<Node> FindPath(List<Node> grid, Node start, Node target){
        List<Node> openList = new List<Node>();
        
        start.gCost = 0;
        start.hCost = Vector2Int.Distance(start.position, target.position);
        start.fCost = Vector2Int.Distance(start.position, target.position);

        openList.Add(start);

        //return null;

        while(openList.Count > 0){

            // Get node with lowest cost
            Node currentNode = null;
            float lowestCost = float.MaxValue;
            for(int i = 0; i < openList.Count; i++){
                if(openList[i].fCost < lowestCost){
                    lowestCost = openList[i].fCost;
                    currentNode = openList[i];
                }
            }

            // Done! Reconstruct and return path
            if(currentNode == target){
                List<Node> path = new List<Node>();
                Node next = currentNode;
                while(next != null){
                    path.Add(next);
                    next = next.parent;
                }
                return path;
            }

            openList.Remove(currentNode);

            // check all neighbours
            for(int y = -1; y <= 1; y++){
                for(int x = -1; x <= 1; x++){
                    Vector2Int neighbourPosition = new Vector2Int(currentNode.position.x + x, currentNode.position.y+y);
                    int index = GetIndex(neighbourPosition.x, neighbourPosition.y);
                    
                    // Check self and OOB
                    if((x == 0 && y == 0) || IsOutOfBounds(neighbourPosition.x, neighbourPosition.y)) continue;
                    
                    Node neighbourNode = grid[index];
                    
                    // Check if wall or visited
                    if(neighbourNode.type == Node.Type.WALL || neighbourNode.visited) continue;

                    /*
                    if(!openList.Contains(neighbourNode)){
                        openList.Add(neighbourNode);
                        neighbourNode.parent = currentNode;
                        neighbourNode.gCost = currentNode.gCost + Vector2Int.Distance(currentNode.position, neighbourPosition);
                        neighbourNode.hCost = Vector2Int.Distance(neighbourPosition, target.position);
                        neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;
                    }
                    */

                    float newGCost = currentNode.gCost + Vector2Int.Distance(currentNode.position, neighbourPosition);
                    if(newGCost < neighbourNode.gCost){
                        neighbourNode.parent = currentNode;
                        neighbourNode.gCost = newGCost;
                        neighbourNode.hCost = Vector2Int.Distance(neighbourPosition, target.position);
                        neighbourNode.fCost = neighbourNode.gCost + neighbourNode.hCost;

                        if(!openList.Contains(neighbourNode))
                            openList.Add(neighbourNode);
                    }
                    
                }
            }
        }

        return null;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
