Shader "Andy/LitVertexColor"
{
    Properties
    {
        _Light ("Light", float) = 0
        _Shade ("Shade", float) = 0
        _MatCap("MatCap", 2D) = "white" {}
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

            #include "Q.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float4 color    : TEXCOORD0;
                float3 normal   : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };
            

            v2f vert (appdata v)
            {
                v2f o;
                float4 vert = mul(unity_ObjectToWorld, v.vertex);
                       vert = Flattened(vert);
                       
                o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, vert));
                
                o.color    = v.color;
                o.normal   = mul(unity_ObjectToWorld, v.normal.xyz);
                o.worldPos = vert;
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.normal.xyz);
                
                float ref   = RefMulti(n);
                float light = Light(n);
                float cone  = Cone(i.worldPos);
                
                //return float4(ColorTest(i.color.rgb * ref * i.color.a + light, cone), 1);
                return float4((ColorTest(i.color.rgb, cone) * ref * i.color.a + light) * cone, 1);
                
                return float4(float3(.5, .5, .5) + light, 1);
            }
            ENDCG
        }
    }
}
