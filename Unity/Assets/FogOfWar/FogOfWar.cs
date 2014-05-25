using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FogOfWar : MonoBehaviour
{
    // INSPECTOR VARIABLES
    public float TargetAlpha = 0.75f;
 
    public float RevSpeed = 0.8f;
    public float HideSpeed = 0.5f;
    public float HideTime = 1f;

    public GameObject Revealer;  

    public int TexSize;

    public int RevInnerRadius;
    public int RevRadius;

	public Color CloudColor;

    public float MovSpeed;

    // PRIVATE VARIABLES
    private Vector3 _lastPos;

	private float _curRevInnerRad;

    private Color[] _colArr;
    private Color[] _colArrCur;
    private Color[] _colArrMask;

    private Texture2D _mainTex;
    private Texture2D[] _perlinTex;

    private int _texWidth;
    private int _texHeight;

    private Dictionary<int,Vector2> _pixToHide;

    private Vector2[] _perlinSpeed;

    // CONST VARIBALES
    private int octaves = 6;
    private float persistence = 0.6f;
    
    private float contrastLow = 0f;
    private float contrastHigh = 1f;    

    // METHODS
    internal void Start()
    {
        _pixToHide = new Dictionary<int,Vector2>();

        _texWidth = (int) transform.localScale.z * TexSize;
        _texHeight = (int) transform.localScale.x * TexSize;

        _mainTex = new Texture2D(_texHeight, _texWidth, TextureFormat.Alpha8, false);

        _colArr = _mainTex.GetPixels(0);
        _colArrCur = _mainTex.GetPixels(0);
        _colArrMask = _mainTex.GetPixels(0);

        for (var i = 0; i < _colArr.Length; i++)
        {
            _colArr [i] = new Color(0, 0, 0, 1 - TargetAlpha);
            _colArrCur[i] = new Color(0, 0, 0, TargetAlpha);
            _colArrMask [i] = new Color(0, 0, 0, 0);
        }
       
        _mainTex.SetPixels(_colArrCur, 0);
        _mainTex.Apply(false);

        for (var i = 0; i < renderer.materials.Length - 1; i++)
        {
            renderer.materials [i].SetTexture("_AlphaMask", _mainTex);
            renderer.materials [i].SetFloat("_DefaultAlpha", 1.0f);
        }

        // Perlin Noises
        _perlinTex = new Texture2D[renderer.materials.Length];
        _perlinSpeed = new Vector2[renderer.materials.Length];

        for (var i = 0; i < renderer.materials.Length; i++)
        {
            _perlinTex [i] = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            _perlinSpeed [i] = new Vector2(Random.Range(-10,10), Random.Range(-10,10));

            GenerateCloudNoise(ref _perlinTex[i], 256, 256, Random.Range(0, 1000), Random.Range(1, 7));
            renderer.materials [i].mainTexture = _perlinTex [i];

            var offVec = new Vector2(Random.Range(-1, 1)/100f, Random.Range(-1, 1)/100f);
            renderer.materials [i].SetVector("_TexOffset", offVec);
        }

        _lastPos = new Vector3(-99, -99, -99);
    }

    internal void Update()
    {
        bool redraw = false;

        var playerPos = Revealer.transform.position;
        var diffPos = playerPos - transform.position;
        
        var width = transform.localScale.z * 10;
        var height = transform.localScale.x * 10;
        
        var localPos = diffPos + new Vector3(width / 2.0f, -height / 2.0f, 0);
        
        var pixPosX = (int)((localPos.x / height) * _texHeight);
        var pixPosY = (int)-((localPos.y / width) * _texWidth);
        var pixPos = new Vector2(pixPosX, pixPosY);

		var radOff = (Mathf.PerlinNoise (Time.time, 0.0f) - 0.5f) / 100;
		_curRevInnerRad = Mathf.Max (RevInnerRadius - 0.2f,
			Mathf.Min (RevInnerRadius + 0.2f, _curRevInnerRad + radOff));

		float inRad = TexSize / _curRevInnerRad;
        float outRad = TexSize / RevRadius;       

        if (playerPos != _lastPos)
        {
            for (var x = pixPosX - outRad; x <= pixPosX + outRad; x++)
                for (var y = pixPosY - outRad; y <= pixPosY + outRad; y++) {
                    var pixInd = (int)(x * _texHeight + y);

                    var posV = new Vector2(x, y);
                    var dist = Vector2.Distance(posV, pixPos);

                    if (dist <= outRad) {
                        var alpha = 0f;

                        if (dist > inRad)
                            alpha = (dist - inRad) / (outRad - inRad);

                        if (pixInd >= 0 && pixInd < _colArr.Length) {
                            _colArrCur[pixInd].a = Mathf.Min(alpha, _colArrCur[pixInd].a);
                            _colArr[pixInd].a = 1 - _colArrCur[pixInd].a;
                            _colArrMask[pixInd].a = Random.Range(1.05f, 1.10f);

                            UniquePush(ref _pixToHide, pixInd, posV);
                        }
                    }
                }

            for (var i = 0; i < renderer.materials.Length; i++)
                renderer.materials[i].SetVector("_PlayerPos", playerPos);

            redraw = true;
            _lastPos = playerPos;
        }

        // hide within a few seconds
        var itemToDelete = new HashSet<int>();

        foreach (var pixToHide in _pixToHide)
        {
            var dist = Vector2.Distance(pixToHide.Value, pixPos);

            if (dist < inRad)
                continue;

			var curArrMask = _colArrMask[pixToHide.Key].a;
			curArrMask -= HideSpeed*Time.deltaTime;

			_colArrCur[pixToHide.Key].a = 1 - (_colArr[pixToHide.Key].a * 
			                                   Mathf.Min(1, _colArrMask[pixToHide.Key].a));

			var maxAlpha = ((dist - inRad) / (outRad - inRad));

                if (_colArrCur[pixToHide.Key].a > maxAlpha) {
                    _colArrCur[pixToHide.Key].a = maxAlpha;
				} else if (_colArrCur[pixToHide.Key].a >= TargetAlpha)
            {
                _colArrCur[pixToHide.Key].a = TargetAlpha;
                _colArr[pixToHide.Key].a = TargetAlpha;
                _colArrMask[pixToHide.Key].a = 0;

                itemToDelete.Add(pixToHide.Key);
            } else {
				_colArrMask[pixToHide.Key].a = curArrMask;

			}

            redraw = true;
        }

        foreach (var item in itemToDelete)
            _pixToHide.Remove(item);

        // redraw texture
        if (redraw)
        {
            _mainTex.SetPixels(_colArrCur, 0);
            _mainTex.Apply(false);
        }

        // moving perlin noises
        for (var i = 0; i < renderer.materials.Length; i++)
        {
            var mainTexOff = renderer.materials [i].GetVector("_TexOffset");

			var offVec = new Vector2(mainTexOff.x + _perlinSpeed [i].x * Time.deltaTime * MovSpeed/10000,
			                         mainTexOff.y + _perlinSpeed [i].y * Time.deltaTime * MovSpeed/10000);

            renderer.materials [i].SetVector("_TexOffset", offVec);
        }
    }

    private static void UniquePush(ref Dictionary<int,Vector2> arr, int elementToAdd, Vector2 position)
    {
        if (!arr.ContainsKey(elementToAdd))
            arr.Add(elementToAdd, position);
    }

    private static void UniquePush(ref Dictionary<int,float> arr, int elementToAdd, float time)
    {
        if (!arr.ContainsKey(elementToAdd))
            arr.Add(elementToAdd, time);
    }   

    void GenerateCloudNoise(ref Texture2D tex, int noiseWidth, int noiseHeight, int seed, int scale) {
        float[,] perlinNoise = PerlinNoise.GeneratePerlinNoise(seed, octaves, persistence, noiseWidth, noiseHeight);
        float noiseValue;

        for(int y = 0; y < noiseWidth; y++) {  
            for(int x = 0; x < noiseHeight; x++) {         
                noiseValue = perlinNoise[x, y];
                noiseValue *= SimplexNoise.SeamlessNoise((float) x / (float) _texWidth, (float) y / (float) _texHeight, scale, scale, 0f);
                
                noiseValue = Mathf.Clamp(noiseValue, contrastLow, contrastHigh + contrastLow) - contrastLow;
                noiseValue = Mathf.Clamp(noiseValue, 0f, 1f);
                
				var brightOff = Random.Range(-0.01f, 0.01f);
				float r = Mathf.Clamp(CloudColor.r + brightOff, 0f, 1f);
				float g = Mathf.Clamp(CloudColor.g + brightOff, 0f, 1f);
				float b = Mathf.Clamp(CloudColor.b + brightOff, 0f, 1f);
                
                tex.SetPixel(x, y, new Color(r, g, b, noiseValue));
                tex.SetPixel(511 - x, y, new Color(r, g, b, noiseValue));
                tex.SetPixel(x, 511 - y, new Color(r, g, b, noiseValue));
                tex.SetPixel(511 - x, 511 - y, new Color(r, g, b, noiseValue));
            }
        }
        
        tex.Apply();
    }
}