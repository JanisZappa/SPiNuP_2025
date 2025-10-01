Shader "AnimalCrossing/Sphere"
{
	Properties 
    { 
        _Radius ("Radius", Range (1, 10000)) = 1
        _MatCap ("MatCap", 2D) = "white" {}
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
			#include "MakeSphere.cginc"

			
			float _Offset;
			
			float4x4 _ToLocal; 
			float4x4 _ToWorldNormal;
		    float4 _Center;
		    
			uniform sampler2D _MatCap;

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
			};
             
			
			
			v2f vert (appdata v)
			{
				v2f o;
				
				float4 before = mul(_ToLocal, mul(unity_ObjectToWorld, v.vertex) - _Center);
				
				sphereOutput sphereOutput = SphereOutput(before, mul(_ToWorldNormal, v.normal));
             
                float dist = before.x * before.x + before.z * before.z;
                o.color  = float4(v.color.x, v.color.y, dist, before.z);
             
                o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, sphereOutput.pos + _Center));
                o.normal = sphereOutput.normal;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			    clip(1 - (i.color.z * 0.0002));
			    
			    float3 normal = normalize(i.normal);
			    
			    fixed2 matCapNormal = mul(UNITY_MATRIX_V, normal).rg;
			    fixed2 matCapUV     = fixed2((matCapNormal.r *  .475 + .5), 
                                             (matCapNormal.g *  .475 + .5));
                                              
                fixed3 matCapMix = tex2D(_MatCap, matCapUV).rgb;
                float matCap = (matCapMix.x + matCapMix.y + matCapMix.z) / 3 - .5;
                                              
				float fog = .9 + pow(saturate(1 - (-0.15 + i.color.w * .025)), 2) * .1;
				float light = .75 + (.5 + dot(normal, normalize(float3(1, .2, 1)) * .75)) * .25;

                return (.6 + pow(i.color.y, 3) * .4) * light * .9 * (.6 + i.color.x * .4) * fog + matCap * .4;
				//return (i.color.x * fog) + matCap * .25;
			}
			ENDCG
		}
	}
}
