Shader "Andy/FinalMatCap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color   ("Color", Color) = (.5, .5, .5, 1)
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#pragma target 3.0

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal      : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);


 				fixed2 matCapNormal = mul(UNITY_MATRIX_V, float4(UnityObjectToWorldNormal(v.normal), 0)).rg;
				o.uv  = fixed2((matCapNormal.r *  .49 + .5), 
					           (matCapNormal.g *  .49 + .5));

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 tex = tex2D(_MainTex, i.uv).rgb;
				fixed3 col = (tex - .5);
				return fixed4(_Color.rgb + col, 1);
			}
			ENDCG
		}
	}
}
