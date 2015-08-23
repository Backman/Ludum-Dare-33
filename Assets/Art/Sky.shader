Shader "Custom/Sky" {
 Properties {
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
                float4 worldPos = mul(_Object2World, i.vertex);
                float frustumHeight = 2.0f * (worldPos.z + _WorldSpaceCameraPos.z) * tan(45 * 0.5);
                float frustumWidth = frustumHeight * (_ScreenParams.x/_ScreenParams.y);


                o.position = mul (UNITY_MATRIX_MVP, i.vertex);
                o.pos = mul(_Object2World, i.vertex);
                o.texcoord0 = i.texcoord0;
                return o;
            }

            fixed4 frag(fragmentInput i) : SV_Target {
//float wave = pow(2,sin(i.pos.x + _Time.y )) + sin(i.pos.y )* 2;
float wave = 0;
            

            float2 skyCoords = float2(fmod(i.pos.x, 1), i.pos.y + wave);

            float4 sky = tex2D(_SkyTex, TRANSFORM_TEX(skyCoords, _SkyTex));
            float4 c = sky;
            //c.rgb = lerp(c.rgb, float3(0.8,0.8,0.9), sealine > 0 ? 1 : 0);
                return float4(c.rgb, 1);
            }
            ENDCG
        }
    }
}
