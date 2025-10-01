Shader "AnimalCrossing/Cylinder"
{
	Properties 
    { 
        _Radius ("Radius", Range (1, 100)) = 1
        _Offset ("Offset", Range (-100, 100)) = 0
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

			float _Radius;
			float _Offset;
			float4x4 _ToWorld;
			float4x4 _ToLocal; 

			#define PI 3.1415926538

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color  : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float  dist: TEXCOORD2;
			};

			float4 ToCylinderPos(float4 pos)
			{
				float circumference = 2 * PI * _Radius;
				float circleLerp =  pos.z / circumference;
				float rad          = 2 * PI * (circleLerp - 0.25);

				float radius = _Radius + pos.y;
		
				return float4(pos.x, -sin(rad) * radius - _Radius, cos(rad) * radius, 1);
				
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 before = mul(_ToLocal, mul(unity_ObjectToWorld, v.vertex));
				float4 worldPos = mul(_ToWorld, ToCylinderPos(before));

				o.dist = before.z;


				o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, worldPos));
				o.color = v.color;
				o.normal = v.normal;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float fog = .35 + pow(saturate(1 - (-2 + i.dist * .15)), 4) * .65;
				float light = .6 + (.5 + dot(normalize(i.normal), normalize(float3(1, 1, 1)) * .75)) * .4;

				return i.color.x * light * fog;
			}
			ENDCG
		}
	}
}
