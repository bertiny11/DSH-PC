using UnityEngine;
using System.Collections.Generic;

public class DrawManager : MonoBehaviour {
    [SerializeField] private GameObject linePrefab; 
    [SerializeField] private CoordinateProjector projector; 
    [SerializeField] private Transform qrAnchor;
    
    private Color _currentColor = Color.red;
    private float _currentWidth = 0.1f;
    

    private LineRenderer _currentLine;
    private List<Vector3> _points = new List<Vector3>();

    void Update() {
        if (Input.GetMouseButtonDown(0)) CreateLine();
        if (Input.GetMouseButton(0) && _currentLine != null) AddPoint();
        if (Input.GetMouseButtonUp(0)) FinalizeStroke();
    }

    void CreateLine() {
        GameObject newLine = Instantiate(linePrefab, qrAnchor);
        _currentLine = newLine.GetComponent<LineRenderer>();
        _currentLine.useWorldSpace = false;
        
        _currentLine.startWidth = _currentWidth;
        _currentLine.endWidth = _currentWidth;
        _currentLine.startColor = _currentColor;
        _currentLine.endColor = _currentColor;
        
        _points.Clear();
        AddPoint();
    }

    void AddPoint() {
        Vector3 point3D = projector.ScreenToQRLocal(Input.mousePosition);
        if (_points.Count == 0 || Vector3.Distance(_points[_points.Count - 1], point3D) > 0.005f) {
            _points.Add(point3D);
            _currentLine.positionCount = _points.Count;
            _currentLine.SetPosition(_points.Count - 1, point3D);
        }
    }

    void FinalizeStroke() {
        if (_points.Count > 1) {
            StrokeData newStroke = new StrokeData();
            newStroke.points = _points.ToArray();
            newStroke.color = _currentColor;
            newStroke.width = _currentWidth;
            newStroke.id = System.Guid.NewGuid().ToString();

            string jsonMessage = JsonUtility.ToJson(newStroke);
            Debug.Log("JSON LISTO: " + jsonMessage);
        }
        _currentLine = null;
        _points.Clear();
    }

    public void SetColorRed() { _currentColor = Color.red; }
    public void SetColorGreen() { _currentColor = Color.green; }
    public void SetColorBlue() { _currentColor = Color.blue; }
    public void SetLineWidth(float newWidth) { _currentWidth = newWidth; }
}