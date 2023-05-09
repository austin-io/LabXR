using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateXR.Avatar;
using UltimateXR.Devices;
using UltimateXR.Manipulation;
using UnityEngine;

public class PaintVR : MonoBehaviour {

    [SerializeField] bool DEBUG_DRAW = false;
    
    [SerializeField] Transform _brushTip;
    [SerializeField] UxrGrabbableObject _grabbableObject;
    [SerializeField] Material _material;
    [SerializeField] Color _color = Color.red;

    [SerializeField] float _stepDistance = 0.02f;
    [SerializeField] float _thickness = 0.02f;

    Vector3 _lastPosition;
    MeshFilter _meshFilter;
    Mesh _mesh;
    
    bool _isDrawing = false;
    List<Vector3> _vertices = new List<Vector3>();
    List<Vector2> _uvs = new List<Vector2>();
    List<int> _triangles = new List<int>();
    List<Color> _colors = new List<Color>();
    
    public void SetColor(Color c){
        _color = c;
    }

    void AddPoints() {

        Vector3 delta = -Vector3.Cross(_lastPosition - _brushTip.position, UxrAvatar.LocalAvatarCamera.transform.position - _brushTip.position).normalized * _thickness;
        /*
        Vector3 _delta = 
            -Vector3.Cross(
                PointPlane(
                    UxrAvatar.LocalAvatarCamera.transform.position, 
                    UxrAvatar.LocalAvatarCamera.transform.forward,
                    _brushTip.position) 
                - PointPlane(
                    UxrAvatar.LocalAvatarCamera.transform.position,
                    UxrAvatar.LocalAvatarCamera.transform.forward, 
                    _lastPosition),
                UxrAvatar.LocalAvatarCamera.transform.forward).normalized * _thickness;
         */

        Vector3 p1 = _brushTip.position + delta;
        Vector3 p2 = _brushTip.position - delta;

        _vertices.Add(p1);
        _vertices.Add(p2);

        _triangles.Add(_vertices.Count - 4);
        _triangles.Add(_vertices.Count - 2);
        _triangles.Add(_vertices.Count - 3);

        _triangles.Add(_vertices.Count - 3);
        _triangles.Add(_vertices.Count - 2);
        _triangles.Add(_vertices.Count - 1);

        _colors.Add(_color);
        _colors.Add(_color);

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.colors = _colors.ToArray();
    }

    void StartDraw() {
        Debug.Log("Start draw");
        _lastPosition = _brushTip.position;

        //Vector3 delta = -_brushTip.up * _thickness;
        Vector3 p1 = _brushTip.position;
        Vector3 p2 = _brushTip.position;

        _vertices.Add(p1);
        _vertices.Add(p2);

        _colors.Add(_color);
        _colors.Add(_color);

    }

    void UpdateDraw() {
        if (Vector3.Distance(_lastPosition, _brushTip.position) < _stepDistance) return;
        AddPoints();
        _lastPosition = _brushTip.position;
    }

    // Start is called before the first frame update
    void Start() {
        GameObject go = Instantiate(new GameObject());
        go.AddComponent<MeshRenderer>().material = _material;
        
        _meshFilter = go.AddComponent<MeshFilter>();
        _meshFilter.mesh = new Mesh();
        
        _mesh = _meshFilter.mesh;
    }

    // Update is called once per frame
    void Update() {
        if(!DEBUG_DRAW){
            UxrGrabManager.Instance.GetGrabbingHand(_grabbableObject, 0, out UxrGrabber grabber);
            if (!_grabbableObject.IsBeingGrabbed 
                || !UxrAvatar.LocalAvatarInput.GetButtonsPress(grabber.Side, UxrInputButtons.Trigger)) {
                _isDrawing = false;
                return;
            }
        }

        if (!_isDrawing) {
            _isDrawing = true;
            StartDraw();
        }

        UpdateDraw();

    }
}