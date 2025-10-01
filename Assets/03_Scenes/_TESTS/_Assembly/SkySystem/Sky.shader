Shader "andy/Sky"
{
	Properties 
	{
		 MainTex ("MainTex", 2D) = "white" {}
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

			uniform sampler2D MainTex;   uniform float4 MainTex_ST;

			struct VertexInput
			{
				float4 vertex : POSITION;
			};

			struct VertexOutput
			{
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.vertex   = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (VertexOutput i) : SV_Target
			{
				 fixed3 worldDir    = normalize(i.worldPos);
                 fixed  gradientPow = saturate(1 - abs(worldDir.y));
                 fixed  powValue    = 1 - (gradientPow * gradientPow * gradientPow);
                 fixed3 skyGradient = tex2D(MainTex, TRANSFORM_TEX( float2(0, powValue), MainTex));
                 return fixed4(skyGradient, 0);
			}
			ENDCG
		}
	}
}
