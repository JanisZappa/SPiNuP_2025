Shader "Unlit/GPUParticle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha One
        Lighting Off ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0

            #include "Q.cginc"
            #define pi 3.14159265
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float2 uv2    : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float4 tint   : TEXCOORD1;
            };
            
            struct Particle
            {
                float2 pos, posMotion;
                float angle;
                float radius;
                int type;
                int searchBound;
                float t;
                float totalT;
    
                float4 dummyA;
                float2 dummyB;
            };

            sampler2D _MainTex;
            //sampler2D _ParticleTex;
            StructuredBuffer<Particle> Part;
            
            //float4 TexSample(float2 uv2)
            //{
           //     return tex2Dlod(_ParticleTex, float4(uv2, 0, 0));
           // }

            v2f vert (appdata v)
            {
                v2f o;
                
                Particle ptc = Part[v.uv2.x];
                
                float r1 = ptc.radius * (1 - pow(1 - (sin(ptc.t / ptc.totalT * pi * 2 - pi * .5) * .5 + .5), 200));
                      r1 *= step(.5, ptc.type);
                
                float p = sin(_Time.y * 20 + v.uv2.x * 30 + v.uv2.y * 30) * .5 + .5;
                float pump = (.9 + .1 * p) * r1;// + 
                         //    ((v.uv2.x + v.uv2.y) * .4 -.2);
               
                float depth = sin(v.uv2.x * 100) * .5 + .5;
                
                float3 pos = float3(ptc.pos - _WorldSpaceCameraPos.xy, round(depth) * .7 - .35);
                
                float4 vt = v.vertex * pump + float4(pos, 0);
                o.vertex = UnityObjectToClipPos(vt);
                o.uv     = v.uv;
                //o.color = float4(sin(sample.w * 10) * .3 + .7, sin(sample.w * 45.31) * .3 + .7, sin(sample.w * 3313.2) * .3 + .7, 1);
                o.tint   = float4(ptc.pos, 0, (.35 + .65 * (1 - depth)) * (.75 + .25 * p));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float cone = .8;// Cone(i.tint.xyz);
                return float4((tex2D(_MainTex, i.uv).xyz * 1.4 * i.tint.w) * cone, 1);
            }
            ENDCG
        }
    }
}
