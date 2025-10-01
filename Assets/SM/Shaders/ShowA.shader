// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "andy/ShowA" 
{
    Properties 
    {
        [KeywordEnum(Off, On)] USETEX ("TexToggle", Float) = 0
        MainTex ("MainTex", 2D) = "white" {}
    }

    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0

            #pragma multi_compile SIMPLE_OFF     SIMPLE_ON
            #pragma multi_compile GREY_OFF       GREY_ON
            #pragma multi_compile DESATURATE_OFF DESATURATE_ON
            #pragma multi_compile USETEX_OFF     USETEX_ON
            
            #pragma multi_compile DEV_OFF        DEV_ON
            #pragma multi_compile TEST_OFF       TEST_ON

            uniform sampler2D MainTex;   uniform float4 MainTex_ST;
            uniform sampler2D Palette;   uniform float4 Palette_ST;
            uniform sampler2D ShadowTex; uniform float4 ShadowTex_ST;
            uniform sampler2D LightTex;  uniform float4 LightTex_ST;
            uniform sampler2D MatCap;    uniform float4 MatCap_ST;


            uniform fixed Side;
            uniform fixed SideMulti;

            uniform fixed SideM1;
            uniform fixed SideM2;

            uniform fixed Wall;
            uniform fixed GamePlane;

            uniform fixed3 SunAngle;
            uniform fixed2 SunFactors;
            uniform fixed  SunPowLerp;

        //  Colors  //
            uniform fixed3 SunColor;
            uniform fixed3 AntiSunColor;
            uniform fixed3 Bounce;
            uniform fixed3 FogColor;
            uniform fixed3 SkyColor;
            uniform fixed3 SunSkyMixColor;
            uniform fixed3 Ambient;
            uniform fixed3 ShadowA;
            uniform fixed3 ShadowB;
            
            uniform fixed3 SunAngleSteep;
            uniform fixed3 ShadowDir;
            uniform fixed  ContactAdd;
            uniform fixed  ContactMultiFactor;

            uniform fixed  ShadowVisibility;

            uniform fixed4 MapA;
            uniform fixed4 MapB;

            uniform fixed  PaletteFactor;
            uniform fixed  PaletteOffset;

            uniform fixed  MatCapXMulti;

            uniform fixed3 SkySize;
            uniform fixed3 SkyCenter;

            uniform fixed  TexPan;

            uniform fixed ViewDirY;
            

            struct VertexInput 
            {
                float4 vertex      : POSITION;
                float3 normal      : NORMAL;
                float4 vertexColor : COLOR;
            };


            struct VertexOutput
            {
                fixed4 vertexColor : COLOR;
            };


            VertexOutput vert (VertexInput v, out float4 outpos : SV_POSITION) 
            {
                VertexOutput o = (VertexOutput)0;
                outpos         = UnityObjectToClipPos(v.vertex);
                o.vertexColor  = v.vertexColor;
  
                return o;
            }


            float4 frag(VertexOutput i, UNITY_VPOS_TYPE vpos : VPOS) : COLOR 
            {
                return i.vertexColor.a;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
