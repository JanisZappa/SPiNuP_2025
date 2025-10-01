Shader "Unlit/Grid"
{
    Properties
    {
        _Size  ("Size", float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha One
        LOD 100
        ZWrite  On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            float _Size;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float pi = 3.14159265359;
                
                float offset = pi * -.5;
                float multi  = 2 / _Size * pi;
                
                float x = pow(1 - pow(1 - (sin(offset + i.uv.x * multi) * .5 + .5), 100000), 5);
                float y = pow(1 - pow(1 - (sin(offset + i.uv.y * multi) * .5 + .5), 100000), 5);
                
                return (1 - (x * y)) * (.2 + i.uv.z * .003);
            }
            ENDCG
        }
    }
}
