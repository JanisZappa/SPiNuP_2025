Shader "Andy/LitVertexColorMorph"
{
    Properties
    {
        _Light ("Light", float) = 0
        _Shade ("Shade", float) = 0
        _MatCap("MatCap", 2D) = "white" {}
        _Detail("Detail Map", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "white" {}
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
            #include "Q.cginc"
            
            
            sampler _Detail, _NormalMap;
            

            struct appdata
            {
                float4 vertex  : POSITION;
                float4 color   : COLOR;
                float4 normal  : NORMAL;
                float3 tangent : TANGENT;
                
                float4 uv  : TEXCOORD0;
                float4 uv2 : TEXCOORD1;
                float4 uv3 : TEXCOORD2;
                float4 uv4 : TEXCOORD3;
                
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float4 color    : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv       : TEXCOORD2;
                
                float3 normal : TEXCOORD3;
                float3 B      : TEXCOORD4;
                float3 T      : TEXCOORD5;
            };


            v2f vert (appdata v)
            {
                v2f o;
                
                float speed = min(.15 * 10 * v.uv3.x, 30);
                float l     = pow(sin(v.uv2.y * -950 + _Time.y * speed) * .5 + .5, 1 + v.uv3.y) * .05;
                
                float amount = l * .18 * 5;
                
                float4 vert = mul(unity_ObjectToWorld, v.vertex);
                       vert.x += v.uv.x  * amount;
                       vert.y += v.uv.y  * amount;
                       vert.z += v.uv2.x * amount;
                       
                       vert = Flattened(vert);
                
                o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, vert));
                
                o.color    = v.color;
                o.worldPos = vert;
                o.uv       = v.uv4.xy;
                
            //  calc Normal, Binormal, Tangent vector in world space
			//  cast 1st arg to 'float3x3' (type of input.normal is 'float3')
				float3 worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
				float3 worldTangent = mul((float3x3)unity_ObjectToWorld, v.tangent);
				
				float3 binormal = cross(v.normal, v.tangent.xyz);
				float3 worldBinormal = mul((float3x3)unity_ObjectToWorld, binormal);

			//  and, set them
				o.normal = normalize(worldNormal);
				o.T = normalize(worldTangent);
				o.B = normalize(worldBinormal);
                
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
            //  obtain a normal vector on tangent space
                float4 t = tex2D(_NormalMap, i.uv * 27);
				float3 tangentNormal = t.xyz;
			//  and change range of values (0 ~ 1)
				tangentNormal = normalize(tangentNormal * 2 - 1);
				tangentNormal.x *= .75;
				tangentNormal.y *= .75;

			//  'TBN' transforms the world space into a tangent space
			//  we need its inverse matrix
			//  Tip : An inverse matrix of orthogonal matrix is its transpose matrix
				float3x3 TBN = float3x3(normalize(i.T), normalize(i.B), normalize(i.normal));
				TBN = transpose(TBN);

			//  finally we got a normal vector from the normal map
				float3 n = mul(TBN, tangentNormal);
                
                float ref   = RefMulti(n);
                      ref   = max(ref, ref);
                float light = Light(n);
                float cone  = Cone(i.worldPos);
                
                float3 tex = tex2D(_Detail, i.uv).xyz;
                
                float3 c   = i.color.rgb;
                return float4((ColorTest(c, cone) * ref * i.color.a  + light) * cone, 1);
                return float4((ColorTest(i.color.rgb, cone) * ref * i.color.a * (.95 + .05 * (1 - pow(1- t.a, 3))) + light) * cone, 1);
            }
            ENDCG
        }
    }
}
