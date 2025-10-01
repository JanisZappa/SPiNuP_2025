Shader "andy/RGBAShadow" 
{
    Properties { _MainTex ("MainTex", 2D) = "black" {} }

    SubShader 
    {
        Pass 
        {
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0

            uniform sampler2D _MainTex;

            struct VertexInput 
            {
                float4 vertex      : POSITION;
                float2 texcoord0   : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput 
            {
                float4 pos         : SV_POSITION;
                float2 uv0         : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.uv0          = v.texcoord0;
                o.vertexColor  = v.vertexColor;
                o.pos          = UnityObjectToClipPos( v.vertex );
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
            {
                fixed shadowLerp = tex2D(_MainTex, i.uv0).r;
                return i.vertexColor * (1- shadowLerp);
            }
            ENDCG
        }
    }
}
