Shader "Andy/Moon"
{
	Properties 
    { 
        _MainTex ("Tex (RGB)", 2D) = "white" {}
    }

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal: NORMAL;
				float4 color: COLOR;
				float2 uv: TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal: TEXCOORD0;
				float4 color : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};

			uniform float3 SunOnMoon;
			sampler2D _MainTex;
			uniform float dotAdd;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.color  = v.color;
				o.uv     = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float light = pow(1 - pow(1 - saturate(dot(SunOnMoon, i.normal)), 6), 1.8);

				fixed4 tex = tex2D (_MainTex, i.uv);
				return light * tex.r + pow(tex.r, 3) * .15 + dotAdd;
			}
			ENDCG
		}
	}
}
