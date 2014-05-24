// FogOfWar shader
// Copyright (C) 2013 Sergey Taraban <http://staraban.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

Shader "Custom/FogOfWar" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _FogRadius ("FogRadius", Float) = 1.0
    _FogMaxRadius("FogMaxRadius", Float) = 0.5
    _PlayerPos ("_PlayerPos", Vector) = (0,0,0,1)
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 200
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Off

    CGPROGRAM
    #pragma surface surf Lambert vertex:vert

    sampler2D _MainTex;
    fixed4     _Color;
    float     _FogRadius;
    float     _FogMaxRadius;
    float4     _PlayerPos;

    struct Input {
        float2 uv_MainTex;
        float2 location;
    };

    void vert(inout appdata_full vertexData, out Input outData) {
        float4 pos = mul(UNITY_MATRIX_MVP, vertexData.vertex);
        float4 posWorld = mul(_Object2World, vertexData.vertex);
        outData.uv_MainTex = vertexData.texcoord;
        outData.location = posWorld.xy;
    }

    void surf (Input IN, inout SurfaceOutput o) {
        float alpha = tex2D(_MainTex, IN.uv_MainTex);
        
        if (length(IN.location - _PlayerPos) < _FogRadius)
			alpha = 0;

        o.Albedo = _Color;
        o.Alpha = alpha;
    }

    ENDCG
}

Fallback "Transparent/VertexLit"
}