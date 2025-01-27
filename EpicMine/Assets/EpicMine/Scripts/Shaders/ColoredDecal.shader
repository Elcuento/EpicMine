// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/ColoredDecal" {
Properties {
    _Color ("Texture Color", Color) = (1,1,1,1)
	_ColorDecal ("Decal Color", Color) = (1,1,1,0)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _DecalTex ("Decal (RGBA)", 2D) = "white" {}
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 250

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
sampler2D _DecalTex;
fixed4 _Color;
fixed4 _ColorDecal;

struct Input {
    float2 uv_MainTex;
    float2 uv_DecalTex;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 d = tex2D(_DecalTex, IN.uv_DecalTex);

	d *= _ColorDecal;
    c *= _Color;

    c.rgb = lerp (c.rgb, d.rgb, d.a);
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}

Fallback "Custom Shaders/ColoredDecal"
}
