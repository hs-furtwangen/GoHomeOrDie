using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FogOfWar : MonoBehaviour
{
    public float TargetAlpha = 0.75f;
 
    public float RevSpeed = 0.8f;
    public float HideSpeed = 0.5f;

    public GameObject Revealer;  

    public int TexSize;
    public int RevRadius;

    private Vector3 _lastPos;

    private Color[] _colArr; 
    private Texture2D _mainTex;

    private int _texWidth;
    private int _texHeight;

    private HashSet<int> _pixToRev;
    private HashSet<int> _pixToHide;

    internal void Start()
    {
        _pixToRev = new HashSet<int>();
        _pixToHide = new HashSet<int>();

        _texWidth = (int) transform.localScale.z * TexSize;
        _texHeight = (int) transform.localScale.x * TexSize;

        _mainTex = new Texture2D(_texHeight, _texWidth, TextureFormat.Alpha8, false);
        _colArr = _mainTex.GetPixels(0);

        for (var i = 0; i < _colArr.Length; i++)
            _colArr[i] = new Color(0, 0, 0, TargetAlpha);
       
        _mainTex.SetPixels(_colArr, 0);
        _mainTex.Apply(false);

        renderer.material.mainTexture = _mainTex;
    }

    internal void Update()
    {
        bool redraw = false;

        var playerPos = Revealer.transform.position;

        if (playerPos != _lastPos)
        {
            var diffPos = playerPos - transform.position;

            var width = transform.localScale.z * 10;
            var height = transform.localScale.x * 10;

            var localPos = diffPos + new Vector3(width / 2.0f, -height / 2.0f, 0);

            var pixPosX = (int)((localPos.x / height) * _texHeight);
            var pixPosY = (int)-((localPos.y / width) * _texWidth);
            var pixPos = new Vector2(pixPosX, pixPosY);

            var radius = TexSize / RevRadius;

            for (var x = pixPosX - radius; x <= pixPosX + radius; x++)
                for (var y = pixPosY - radius; y <= pixPosY + radius; y++) {
                    var dist = Vector2.Distance(new Vector2(x, y), pixPos);

                    if (dist <= radius) {
                        var pixInd = (int)(x * _texHeight + y);

                        if (pixInd >= 0 && pixInd < _colArr.Length)
                            UniquePush(ref _pixToRev, pixInd);
                    }
                }

            renderer.material.SetVector("_PlayerPos", playerPos);

            redraw = true;
            _lastPos = playerPos;
        }

        // reveal within a few seconds
        var itemToDelete = new HashSet<int>();

        foreach (var pixToRev in _pixToRev)
        {
            _colArr[pixToRev].a -= RevSpeed*Time.deltaTime;
            
            if (_colArr[pixToRev].a <= 0)
            {
                _colArr [pixToRev].a = Random.Range(-0.1f, 0);

                itemToDelete.Add(pixToRev);
                UniquePush(ref _pixToHide, pixToRev);
            }
            
            redraw = true;
        }

        foreach (var item in itemToDelete)
            _pixToRev.Remove(item);

        // hide after a few seconds
        itemToDelete.Clear();

        foreach (var pixToHide in _pixToHide)
        {
            _colArr[pixToHide].a += HideSpeed*Time.deltaTime;

            if (_colArr[pixToHide].a >= TargetAlpha)
            {
                _colArr[pixToHide].a = TargetAlpha;
                itemToDelete.Add(pixToHide);
            }

            redraw = true;
        }

        foreach (var item in itemToDelete)
            _pixToHide.Remove(item);

        // redraw texture
        if (redraw)
        {
            _mainTex.SetPixels(_colArr, 0);
            _mainTex.Apply(false);
        }
    }

    private static void UniquePush(ref HashSet<int> arr, int elementToAdd)
    {
        if (!arr.Contains(elementToAdd))
            arr.Add(elementToAdd);
    }
}