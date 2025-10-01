Shader "andy/RGBA" 
{
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
            #pragma target 3.5

            struct VertexInput 
            {
                float4 vertex      : POSITION;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput 
            {
                float4 pos         : SV_POSITION;
                float4 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor  = v.vertexColor;
                o.pos          = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
            {
                return i.vertexColor * i.vertexColor;
            }
            ENDCG
        }
    }
}
