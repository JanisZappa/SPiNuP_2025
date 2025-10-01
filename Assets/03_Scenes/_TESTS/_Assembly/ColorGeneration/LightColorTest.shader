
Shader "Andy/LightColorTest" 
{
    Properties 
    {
        ColorA ("ColorA", Color) = (1, 1, 1, 1)
        ColorB ("ColorB", Color) = (0, 0, 0, 1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0


            uniform fixed3 SunDir;
            uniform fixed4 ColorA;
            uniform fixed4 ColorB;


            struct VertexInput 
            {
                float4 vertex      : POSITION;
                float3 normal      : NORMAL;
                float4 vertexColor : COLOR;
            };


            struct VertexOutput
            {
                fixed3 normal: TEXCOORD0;
            };


            VertexOutput vert (VertexInput v, out float4 outpos : SV_POSITION) 
            {
                VertexOutput o = (VertexOutput)0;
                        outpos = UnityObjectToClipPos(v.vertex);
                      o.normal = UnityObjectToWorldNormal(v.normal);
  
                return o;
            }


            float4 frag(VertexOutput i, UNITY_VPOS_TYPE vpos : VPOS) : COLOR 
            {
                fixed  sunDot  = dot(SunDir, i.normal);
                fixed  sunSign = sign(sunDot);
                fixed  sunAbs  = abs(sunDot);
                       sunDot  = saturate((1 - pow(1 - sunAbs, 1000)) * sunSign * .5 + .5);

                return fixed4(ColorB.r + (ColorA.r - ColorB.r) * sunDot,
                              ColorB.g + (ColorA.g - ColorB.g) * sunDot,
                              ColorB.b + (ColorA.b - ColorB.b) * sunDot, 1);
            }

            ENDCG
        }
    }
}
