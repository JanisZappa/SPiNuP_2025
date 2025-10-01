// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33434,y:32576,varname:node_3138,prsc:2|emission-5745-OUT;n:type:ShaderForge.SFN_Vector4Property,id:3865,x:31976,y:32759,ptovrint:False,ptlb:SunAngle,ptin:SunAngle,varname:_SunAngle,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5,v2:0.5,v3:0.5,v4:1;n:type:ShaderForge.SFN_NormalVector,id:6583,x:32275,y:32878,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:898,x:32275,y:32706,varname:node_898,prsc:0,dt:1|A-3865-XYZ,B-6583-OUT;n:type:ShaderForge.SFN_Multiply,id:4072,x:32475,y:32636,varname:node_4072,prsc:2|A-7212-RGB,B-898-OUT;n:type:ShaderForge.SFN_Color,id:7212,x:31976,y:32534,ptovrint:False,ptlb:SunColor,ptin:SunColor,varname:_SunColor,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_VertexColor,id:480,x:32661,y:32806,varname:node_480,prsc:2;n:type:ShaderForge.SFN_Add,id:7197,x:32661,y:32663,varname:node_7197,prsc:2|A-4072-OUT,B-1776-RGB;n:type:ShaderForge.SFN_AmbientLight,id:1776,x:32475,y:32756,varname:node_1776,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9213,x:32850,y:32663,varname:node_9213,prsc:2|A-7197-OUT,B-480-RGB;n:type:ShaderForge.SFN_FragmentPosition,id:5702,x:32128,y:33734,varname:node_5702,prsc:2;n:type:ShaderForge.SFN_InverseLerp,id:553,x:32369,y:33563,varname:node_553,prsc:2|A-3849-OUT,B-60-OUT,V-5702-Z;n:type:ShaderForge.SFN_ValueProperty,id:3849,x:31998,y:33488,ptovrint:False,ptlb:Wall,ptin:Wall,varname:_Wall_Front_copy,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Clamp01,id:7298,x:32538,y:33563,varname:node_7298,prsc:2|IN-553-OUT;n:type:ShaderForge.SFN_Color,id:609,x:32369,y:33330,ptovrint:False,ptlb:FogColor,ptin:FogColor,varname:node_3509,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:3507,x:32685,y:33359,varname:node_3507,prsc:2|A-609-RGB,B-609-A,C-3647-OUT,D-8613-OUT;n:type:ShaderForge.SFN_Vector1,id:3647,x:32369,y:33488,varname:node_3647,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Power,id:8613,x:32756,y:33598,varname:node_8613,prsc:2|VAL-7298-OUT,EXP-4823-OUT;n:type:ShaderForge.SFN_Vector1,id:4823,x:32538,y:33697,varname:node_4823,prsc:2,v1:2;n:type:ShaderForge.SFN_Set,id:8291,x:32858,y:33359,varname:WallFog,prsc:2|IN-3507-OUT;n:type:ShaderForge.SFN_Add,id:5745,x:33119,y:32717,varname:node_5745,prsc:2|A-9213-OUT,B-9655-OUT;n:type:ShaderForge.SFN_Get,id:9655,x:33098,y:32849,varname:node_9655,prsc:2|IN-8291-OUT;n:type:ShaderForge.SFN_Multiply,id:60,x:32155,y:33575,varname:node_60,prsc:2|A-3849-OUT,B-1241-OUT;n:type:ShaderForge.SFN_Vector1,id:1241,x:31998,y:33604,varname:node_1241,prsc:2,v1:-1;pass:END;sub:END;*/

Shader "andy/Vertex_WallLerp" {
    Properties {
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
            #pragma target 2.0
            uniform fixed4 SunAngle;
            uniform fixed4 SunColor;
            uniform fixed Wall;
            uniform fixed4 FogColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos        : SV_POSITION;
                float4 posWorld   : TEXCOORD0;
                float3 normalDir  : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 WallFog = (FogColor.rgb*FogColor.a*0.7*pow(saturate(((i.posWorld.b-Wall)/((Wall*(-1.0))-Wall))),2.0));
                float3 emissive = ((((SunColor.rgb*max(0,dot(SunAngle.rgb,i.normalDir)))+UNITY_LIGHTMODEL_AMBIENT.rgb)*i.vertexColor.rgb)+WallFog);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
