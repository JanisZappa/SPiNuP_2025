Shader "Andy/MatCapBubble"
{
    Properties
    {
        _MatCap("MatCap", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha One
        Lighting Off ZWrite Off
        

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
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
                
                return pow(MatCapX(n), 1.5) * 1.5 * (.5 + .5 * Cone(i.worldPos));
            }
            ENDCG
        }
    }
}
