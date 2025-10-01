Shader "Andy/ringtest"
{
	Properties
	{
	    _scale("Radius" , float) = 1
	    _blob("Size", float) = 1
	    _lerp("Lerp", float) = 1
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
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float tint : TEXCOORD0;
			};

			float _scale;
			float _blob;
			float _lerp;
			
			v2f vert (appdata v)
			{
				v2f o;
				float3 center = v.vertex - v.normal * v.uv.x;
				
				float l = saturate(_lerp);

				float multi =  ((    abs(    (v.uv.y - .5)   ) * 2) - l) * ((1 - l) / 1);
				      multi = 1 - pow(1 - pow(clamp(multi, 0, 1), 2), 100);

				//float multi =  smoothstep(0, 1, saturate( (abs((v.uv.y - .5)) * 2) - l) * ((1 - l) / 1));
				float3 pos = center * _scale + v.normal * v.uv.x * _blob * multi;
				o.vertex = UnityObjectToClipPos(pos);
				o.tint = .5 + saturate(dot(float3(0, 1, 0), v.normal) * .5 + .5) * .5;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return i.tint;
			}
			ENDCG
		}
	}
}
