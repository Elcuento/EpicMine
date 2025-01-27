// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/MonsterBlended" {
Properties {
    _Color ("Texture Color", Color) = (1,1,1,1)
	_ColorHit ("Hit Color", Color) = (1,1,1,0)
	_ColorAcid ("Acid Color", Color) = (1,1,1,0)
	_ColorFrozen ("Frozen Color", Color) = (1,1,1,0)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _HitTex ("Hit (RGBA)", 2D) = "white" {}
	_AcidTex ("Acid (RGBA)", 2D) = "white" {}
	_FrozenTex ("Frozen (RGBA)", 2D) = "white" {}
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 250

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
sampler2D _HitTex;
sampler2D _AcidTex;
sampler2D _FrozenTex;
fixed4 _Color;
fixed4 _ColorHit;
fixed4 _ColorAcid;
fixed4 _ColorFrozen;

struct Input {
    float2 uv_MainTex;
    float2 uv_HitTex;
	float2 uv_AcidTex;
	float2 uv_FrozenTex;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 a = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 b = tex2D(_HitTex, IN.uv_HitTex);
	fixed4 c = tex2D(_AcidTex, IN.uv_AcidTex);
	fixed4 d = tex2D(_FrozenTex, IN.uv_FrozenTex);

	a *= _Color;
	d *= _ColorFrozen;
    c *= _ColorAcid;
	b *= _ColorHit;

    d.rgb = lerp (a.rgb, d.rgb, d.a);
	o.Albedo = d.rgb;
	o.Alpha = d.a;
  
    c.rgb = lerp (d.rgb, c.rgb, c.a);
	o.Albedo = c.rgb;
	o.Alpha = c.a;

    b.rgb = lerp (c.rgb, b.rgb, b.a);
	o.Albedo = b.rgb;
	o.Alpha = b.a;

}
ENDCG
}

Fallback "Custom Shaders/MonsterBlended"
}
