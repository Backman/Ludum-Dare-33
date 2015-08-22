﻿Shader "Custom/Background" {
 Properties {
        _BackgroundTex ("Background", 2D) = "white" {}
        _SkyTex ("Sky", 2D) = "white" {}
        _SealineSize ("Sealine", Float) = 10
    }
    SubShader {
        Pass {
        ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _BackgroundTex;
            float4 _BackgroundTex_ST;
            sampler2D _SkyTex;
            float4 _SkyTex_ST;
            float _SealineSize;

            struct vertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };

            struct fragmentInput{
                float4 position : SV_POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 pos : TEXCOORD1;
            };

            fragmentInput vert(vertexInput i){
                fragmentInput o;
                o.position = mul (UNITY_MATRIX_MVP, i.vertex);
                o.pos = mul(_Object2World, i.vertex);
                o.texcoord0 = i.texcoord0;
                return o;
            }
            fixed4 frag(fragmentInput i) : SV_Target {

//float wave = pow(2,sin(i.pos.x + _Time.y )) + sin(i.pos.y )* 2;
float wave = 0;
            
            float2 skyCoords = float2(i.pos.x, i.pos.y + wave);
            float2 bgCoords = float2(i.pos.x, i.pos.y - 0.02 + wave);

            float4 bg = tex2D(_BackgroundTex, TRANSFORM_TEX(bgCoords, _BackgroundTex));
            float4 sky = tex2D(_SkyTex, TRANSFORM_TEX(skyCoords, _SkyTex));
            float4 c = lerp(bg, sky, (i.pos.y + wave)> 0 ? 1 : 0);
            float sealine = 1 - abs(i.pos.y + wave) * _SealineSize;
            c.rgb = lerp(c.rgb, float3(0.8,0.8,0.9), saturate(sealine));
            //c.rgb += sealine.xxx;
            //return float4(i.pos.xy, 0, 1);
                return float4(c.rgb, 1);
            }
            ENDCG
        }
        Pass {
            Stencil {
                Ref 2
                Comp always
                Pass replace
            }
            ZWrite Off
            ColorMask 0
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float4 position : TEXCOORD0;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.position = mul(_Object2World, v.vertex);
                return o;
            }
            half4 frag(v2f i) : SV_Target {
                  clip(i.position.y );
                return half4(1,0,0,1);
            }
            ENDCG
        }
        
    }
}
