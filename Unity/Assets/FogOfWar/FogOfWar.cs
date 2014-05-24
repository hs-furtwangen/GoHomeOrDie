using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
  /*  private const float InitialRevealerCircleSmoothness = 0.05f;
    private const float RevealerCircleSmoothness = 0.4f;

    public float RevealerRange = 1.0f;
    public float RevealSpeed = 0.5f;
    public float HideSpeed = 0.5f;
    public float TargetAlpha = 0.75f;

    public int XLength = 10;
    public int YLength = 10;
    public float GridSize = 3.0f;
    
    public Material NewMaterial;
    public GameObject Revealer;

    public LayerMask RayLayer;

    private Color[] _colors;
    private Mesh _mesh;

    private Vector3 _lastPos;
    private float _maxDist;

    private Dictionary<int, Vector3> _vertices;
    private List<int> _vertToHide;
    private List<int> _vertToRev;*/

    private Texture2D _mainTex;

    public float GridSize;

    internal void Start()
    {
      /*  _vertices = new Dictionary<int,Vector3>();
        _vertToHide = new List<int>();
        _vertToRev = new List<int>();

        InitMesh();
        InitialReveal();

        _maxDist = 25;*/

        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var width = transform.localScale.x;
        var height = transform.localScale.y;

        _mainTex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        
        // set the pixel values
        texture.SetPixel(0, 0, Color(1.0, 1.0, 1.0, 0.5));
        texture.SetPixel(1, 0, Color.clear);
        texture.SetPixel(0, 1, Color.white);
        texture.SetPixel(1, 1, Color.black);
        
        // Apply all SetPixel calls
        texture.Apply();
        
        // connect texture to material of GameObject this script is attached to
        renderer.material.mainTexture = texture;



        var texture : Texture2D = Instantiate(renderer.material.mainTexture);
        renderer.material.mainTexture = texture;
        // colors used to tint the first 3 mip levels
        var colors = new Color[3];
        colors[0] = Color.red;
        colors[1] = Color.green;
        colors[2] = Color.blue;
        var mipCount = Mathf.Min( 3, texture.mipmapCount );
        // tint each mip level
        for( var mip = 0; mip < mipCount; ++mip ) {
            var cols = texture.GetPixels( mip );
            for( var i = 0; i < cols.Length; ++i ) {
                cols[i] = Color.Lerp( cols[i], colors[mip], 0.33 );
            }
            texture.SetPixels( cols, mip );
        }
        // actually apply all SetPixels, don't recalculate mip levels
        texture.Apply( false );
    }

    private void InitMesh()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
     /*   _mesh.Clear();

        // vertices -----------------------------
        var verts = new List<Vector3>();

        for (var y = -YLength/2.0f; y < YLength/2.0f; ++y)
            for (var x = -XLength/2.0f; x < XLength/2.0f; ++x) {
                var vertPos = new Vector3(x*GridSize, y*GridSize, 0);
                var globPos = transform.TransformPoint(vertPos);

                verts.Add(vertPos);
                _vertices.Add(verts.Count - 1, globPos);
            }

        _mesh.vertices = verts.ToArray();

        // uv's ---------------------------------
        var uvs = new Vector2[verts.Count];

        for (int i = 0; i < uvs.Length; i++)
            uvs[i] = new Vector2(verts[i].x, verts[i].y);

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
        _mesh.RecalculateBounds();*/

        // vertex colors -------------------------
        var vertices = _mesh.vertices;
        _colors = new Color[vertices.Length];

        for (var i = 0; i < vertices.Length; i++)
            _colors[i] = new Color(0, 0, 0, TargetAlpha);

        _mesh.colors = _colors;

        renderer.castShadows = false;
        renderer.receiveShadows = false;
        renderer.material = NewMaterial;

     /*   var meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = _mesh;*/
    }

    private void InitialReveal()
    {
        var x = Revealer.transform.position.x;
        var y = Revealer.transform.position.y;
        
        RevealFoWAt(x, y, true);

        for (var rangeStep = 0.0f; rangeStep <= RevealerRange; rangeStep += 0.2f)
            for (var n = 0.0f; n <= 2*Mathf.PI; n += InitialRevealerCircleSmoothness)
                RevealFoWAt(x + (Mathf.Cos(n)*rangeStep), y + (Mathf.Sin(n)*rangeStep), true);

        _lastPos = Revealer.transform.position;
    }

    internal void Update()
    {
        renderer.material.SetVector("_PlayerPos", Revealer.transform.position);

     /*   var x = Revealer.transform.position.x;
        var y = Revealer.transform.position.y;

        var lastPos = new Vector3(_lastPos.x, _lastPos.y);
        if (new Vector3(x, y, 0) != lastPos)
        {
            for (var i = 0.0f; i <= 2*Mathf.PI; i += RevealerCircleSmoothness/RevealerRange)
                RevealFoWAt(x + (Mathf.Cos(i) * RevealerRange), y + (Mathf.Sin(i) * RevealerRange), false);

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
            _mesh.colors = _colors;*/
    }

    private void RevealFoWAt(float x, float y, bool quickReveal)
    {
        RaycastHit hit;
        var deepRay = new Ray(new Vector3(x, y, -5), new Vector3(0, 0, 1));

        if (!Physics.Raycast(deepRay, out hit, 15, RayLayer))
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

    private static void UniquePush(ref List<int> arr, int elementToAdd)
    {
        if (!arr.Contains(elementToAdd))
            arr.Add(elementToAdd);
    }
}