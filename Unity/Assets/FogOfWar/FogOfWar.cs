using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    private const float RaycastHeight = 50.0f;
    private const float InitialRevealerCircleSmoothness = 0.1f;
    private const float RevealerCircleSmoothness = 4.75f;

    public float LosHeight = 5.0f;
    public float RevealerRange = 1.0f;
    public float RevealSpeed = 0.5f;
    public float HideSpeed = 0.5f;
    public float TargetAlpha = 0.75f;

    public int XLength = 10;
    public int YLength = 10;
    public float GridSize = 3.0f;
    
    public Material NewMaterial;
    public GameObject Revealer;

    private Color[] _colors;
    private Mesh _mesh;

    private Vector3 _lastPos;
    private float _maxDist;

    private Dictionary<int, Vector3> _vertices;
    private List<int> _vertToHide;
    private List<int> _vertToRev;

    private Vector3 AddVert(Vector3 pos, float x, float z)
    {
        RaycastHit hitInfo;
        LayerMask layerMask = 1 << 10;

        Physics.Raycast(new Vector3(x + pos.x, RaycastHeight, z + pos.z), -Vector3.up, out hitInfo, Mathf.Infinity,
            layerMask);

        return new Vector3(x, hitInfo.point.y, z);
    }

    internal void Start()
    {
        _vertices = new Dictionary<int,Vector3>();
        _vertToHide = new List<int>();
        _vertToRev = new List<int>();

        InitMesh();
        InitialReveal();

        _maxDist = 25;
    }

    private void InitMesh()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _mesh.Clear();

        // vertices -----------------------------
        var verts = new List<Vector3>();
        var pos = transform.position;

        for (int y = 0; y < YLength; ++y)
            for (int x = 0; x < XLength; ++x) {
            var vertPos = AddVert(pos, (x*GridSize), (y*GridSize));

            verts.Add(vertPos);
            _vertices.Add(verts.Count - 1, transform.TransformPoint(vertPos));
        }

        _mesh.vertices = verts.ToArray();

        // uv's ---------------------------------
        var uvs = new Vector2[verts.Count];

        for (int i = 0; i < uvs.Length; i++)
            uvs[i] = new Vector2(verts[i].x, verts[i].z);

        _mesh.uv = uvs;

        // triangles ----------------------------
        var tris = new List<int>();

        for (var y = 0; y < YLength - 1; ++y)
            for (var x = 0; x < XLength - 1; ++x)
            {
                var vertHere = x + (y*XLength);
                var vertAbove = x + ((y + 1)*XLength);

                tris.Add(vertHere);
                tris.Add(vertAbove);
                tris.Add(vertAbove + 1);

                tris.Add(vertHere);
                tris.Add(vertAbove + 1);
                tris.Add(vertHere + 1);
            }

        _mesh.triangles = tris.ToArray();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        // vertex colors -------------------------
        var vertices = _mesh.vertices;
        _colors = new Color[vertices.Length];

        for (var i = 0; i < vertices.Length; i++)
            _colors[i] = new Color(0, 0, 0, TargetAlpha);

        _mesh.colors = _colors;

        renderer.castShadows = false;
        renderer.receiveShadows = false;
        renderer.material = NewMaterial;

        gameObject.AddComponent<MeshCollider>();
        var meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = _mesh;
    }

    private void InitialReveal()
    {
        var x = Revealer.transform.position.x;
        var z = Revealer.transform.position.z;
        
        RevealFoWAt(x, z, true);
        
        for (var rangeStep = 0.0f; rangeStep <= RevealerRange; rangeStep += 5)
            for (var n = 0.0f; n <= 2*Mathf.PI; n += InitialRevealerCircleSmoothness)
                RevealFoWAt(x + (Mathf.Cos(n)*rangeStep), z + (Mathf.Sin(n)*rangeStep), true);

        _lastPos = Revealer.transform.position;
    }

    internal void Update()
    {
        var x = Revealer.transform.position.x;
        var z = Revealer.transform.position.z;

        var lastPos = new Vector3(_lastPos.x, 0, _lastPos.z);
        if (new Vector3(x, 0, z) != lastPos)
        {
            for (var i = 0.0f; i <= 2*Mathf.PI; i += RevealerCircleSmoothness/RevealerRange)
                RevealFoWAt(x + (Mathf.Cos(i) * RevealerRange), z + (Mathf.Sin(i) * RevealerRange), false);

            _lastPos = Revealer.transform.position;
        }
       
        // reveal witihin a few (milli)seconds
        bool reDraw = false;

        for (var i = 0; i < _vertToRev.Count; ++i)
        {
            var vertIdxToReveal = _vertToRev[i];
            
            _colors[vertIdxToReveal].a -= RevealSpeed*Time.deltaTime;

            if ((_colors[vertIdxToReveal].a <= 0.0f) &&
                (Vector3.Distance(_vertices[vertIdxToReveal], _lastPos) > _maxDist))
            {
                _colors[vertIdxToReveal].a = Random.Range(-1.5f, -0.5f);

                _vertToRev.RemoveAt(i);
                i = Mathf.Max(i - 1, 0);

                UniquePush(ref _vertToHide, vertIdxToReveal);
            }
            
            reDraw = true;
        }
        
        // hide after a few seconds
        for (var i = 0; i < _vertToHide.Count; ++i)
        {
            var vertIdxToHide = _vertToHide[i];
            
            _colors[vertIdxToHide].a += HideSpeed*Time.deltaTime;
            
            if (_colors[vertIdxToHide].a >= TargetAlpha)
            {
                _colors[vertIdxToHide].a = TargetAlpha;

                _vertToHide.RemoveAt(i);
                i = Mathf.Max(i - 1, 0);
            }
            
            reDraw = true;
        }

        if (reDraw)
            _mesh.colors = _colors;
    }

    private void RevealFoWAt(float x, float z, bool quickReveal)
    {
        RaycastHit hit;
        LayerMask layerMask = 1 << 8;
        
        if (!Physics.Raycast(new Vector3(x, RaycastHeight, z), -Vector3.up, out hit, Mathf.Infinity, layerMask))
            return;
        
        var meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;
        
        var losOrigin = transform.position + new Vector3(0, LosHeight, 0);
        var losDir = (hit.point + new Vector3(0, 3, 0)) - losOrigin;
        var losDist = losDir.magnitude;
        losDir.Normalize();
        
        RaycastHit losHit;
        LayerMask losLayerMask = (1 << 0) + (1 << 10);
        if (Physics.Raycast(losOrigin, losDir, out losHit, losDist, losLayerMask))
            return;
        
        var triangles = _mesh.triangles;
        
        // get which vertices were hit
        var p0 = triangles[hit.triangleIndex*3 + 0];
        var p1 = triangles[hit.triangleIndex*3 + 1];
        var p2 = triangles[hit.triangleIndex*3 + 2];
        
        AddVertToReveal(p0, quickReveal);
        AddVertToReveal(p1, quickReveal);
        AddVertToReveal(p2, quickReveal);
    }

    private void AddVertToReveal(int vertIdx, bool quickReveal)
    {
        if (quickReveal)
            _colors[vertIdx].a = 0.0f;

        UniquePush(ref _vertToRev, vertIdx);
    }

    private void UniquePush(ref List<int> arr, int elementToAdd)
    {
        if (arr.Contains(elementToAdd))
            return;

        arr.Add(elementToAdd);
    }

    private bool IsVertRevealed(int vertIdx)
    {
        return (_colors[vertIdx].a < TargetAlpha);
    }
}