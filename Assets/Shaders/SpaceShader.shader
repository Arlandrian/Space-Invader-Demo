// Made with shader graph then ported as shader
// By Numan Kemal Kazancı

Shader "Skybox/SpaceShader"
{
    Properties
    {
		_NoiseScale1("Noise Scale_1", Range(250,2000)) = 750
		_NoiseScale2("Noise Scale_2", Range(250,2000)) = 750

		_Edge1("Edge_1", Range(0,1)) = 0.78
		_Edge2("Edge_2", Range(0,1)) = 0.75

		_EdgeBgMin("EdgeBg Min", Range(0.2,5)) = 0.61
		_EdgeBgMax("EdgeBg Min", Range(0.2,5)) = 3.03

		_FlowSpeed1("Flow Speed_1", Range(0,4)) = 0.05
		_FlowSpeed2("Flow Speed_2", Range(0,4)) = 0.002
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Cull Off ZWrite Off

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"
			#include "unity_simple_noise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


			float _FlowSpeed1;
			float _FlowSpeed2;

			float _NoiseScale1;
			float _NoiseScale2;

			float _Edge1;
			float _Edge2;

			float _EdgeBgMin;
			float _EdgeBgMax;

			//void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)

			//void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				
				float2 offsetUV1,offsetUV2;
				Unity_TilingAndOffset_float(i.uv, float2(1, 1), fixed2(0, _Time.x* _FlowSpeed1), offsetUV1);
				Unity_TilingAndOffset_float(i.uv, float2(1,1), fixed2(0, _Time.x* _FlowSpeed2), offsetUV2);

				float noise1,noise2;
				Unity_SimpleNoise_float(offsetUV1, _NoiseScale1, noise1);
				//step = step(float4 Edge, float4 In)
				float edge1 = step(_Edge1,noise1);

				Unity_SimpleNoise_float(offsetUV2, _NoiseScale2, noise2);
				float edge2 = step(_Edge2, noise2);

				float bgStars =  smoothstep(_EdgeBgMin, _EdgeBgMax, noise2);

				float col = edge1 + edge2 + bgStars;

				float color = float4(col, col, col,1.0);
				

                return color;
            }
            ENDCG
        }
    }
}
