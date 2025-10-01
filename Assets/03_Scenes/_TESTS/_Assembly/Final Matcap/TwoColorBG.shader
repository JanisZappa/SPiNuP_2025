Shader "Andy/TwoColorBG"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
		_ColorA   ("ColorA", Color) = (.5, .5, .5, 1)
		_ColorB   ("ColorB", Color) = (.5, .5, .5, 1)
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

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _ColorA;
			float4 _ColorB;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv     : TEXCOORD0;
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
				o.uv     =  TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed lerpValue = tex2D(_MainTex, i.uv).r;
				fixed3 col = lerp(_ColorA.rgb, _ColorB.rgb, lerpValue);
				return fixed4(col, 1);
			}
			ENDCG
		}
	}
}
