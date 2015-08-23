Shader "Sprites/Foreground Creature"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
    _BlinkColor ("Blink Color", Color) = (0,0,0,0)
    _SeaTint ("SeaTint", Color) = (0,0,0,0)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
    
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 pos  : TEXCOORD1;
			};
			
			fixed4 _Color;
      fixed4 _BlinkColor;
      fixed4 _SeaTint;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
        OUT.pos = mul(_Object2World, IN.vertex);
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;

float4 Overlay (float4 cBase, float4 cBlend)
{
	float4 cNew;
	if (cBase.r > .5) { cNew.r = 1 - (1 - 2 * (cBase.r - .5)) * (1 - cBlend.r); }
	else { cNew.r = (2 * cBase.r) * cBlend.r; }
	
	if (cBase.g > .5) { cNew.g = 1 - (1 - 2 * (cBase.g - .5)) * (1 - cBlend.g); }
	else { cNew.g = (2 * cBase.g) * cBlend.g; }
	
	if (cBase.b > .5) { cNew.b = 1 - (1 - 2 * (cBase.b - .5)) * (1 - cBlend.b); }
	else { cNew.b = (2 * cBase.b) * cBlend.b; }
	
	cNew.a = 1.0;
	return cNew;
}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
        if (IN.pos.y < 0)
        {
            float sealine = abs(IN.pos.y) * 5;
          c.rgb = lerp(c.rgb, lerp(c.rgb, Overlay(c.rgb,_SeaTint.rgb), _SeaTint.a), saturate(sealine + IN.pos.y));
          c.rgb = lerp(c.rgb, float3(0.8, 0.8, 0.9), (1 - sealine) > 0 ? sealine * 0.7 : 0);
        }
        c.rgb = lerp(c.rgb, _BlinkColor.rgb, _BlinkColor.a);
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
