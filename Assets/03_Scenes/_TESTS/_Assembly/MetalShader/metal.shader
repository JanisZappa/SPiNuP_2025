// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:1,fgcb:1,fgca:1,fgde:0.01,fgrn:109.36,fgrf:275.18,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33824,y:32670,varname:node_3138,prsc:2|emission-9044-OUT;n:type:ShaderForge.SFN_Tex2d,id:1756,x:32441,y:32928,ptovrint:False,ptlb:Reflection,ptin:_Reflection,varname:_Reflection,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9598-OUT;n:type:ShaderForge.SFN_NormalVector,id:8115,x:31686,y:32833,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:24,x:32089,y:32951,varname:node_24,prsc:2,dt:4|A-8115-OUT,B-3681-OUT;n:type:ShaderForge.SFN_Vector1,id:2542,x:32089,y:32877,varname:node_2542,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:9598,x:32266,y:32928,varname:node_9598,prsc:0|A-2542-OUT,B-24-OUT;n:type:ShaderForge.SFN_Vector3,id:5151,x:31686,y:33115,varname:node_5151,prsc:0,v1:0,v2:-1,v3:0;n:type:ShaderForge.SFN_ViewVector,id:9843,x:31686,y:32987,varname:node_9843,prsc:2;n:type:ShaderForge.SFN_Add,id:3681,x:31899,y:33037,varname:node_3681,prsc:2|A-9843-OUT,B-5151-OUT;n:type:ShaderForge.SFN_Add,id:9812,x:32629,y:32796,varname:node_9812,prsc:0|A-5265-OUT,B-1756-R;n:type:ShaderForge.SFN_Fresnel,id:4047,x:32129,y:32669,varname:node_4047,prsc:0|EXP-7729-OUT;n:type:ShaderForge.SFN_Vector1,id:7729,x:31992,y:32772,varname:node_7729,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:5265,x:32329,y:32669,varname:node_5265,prsc:2|A-8239-OUT,B-4047-OUT;n:type:ShaderForge.SFN_Vector1,id:8239,x:32129,y:32609,varname:node_8239,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Vector4Property,id:9391,x:32572,y:32429,ptovrint:False,ptlb:_SunAngle,ptin:_SunAngle,varname:__SunAngle,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5,v2:0.5,v3:0.5,v4:1;n:type:ShaderForge.SFN_Add,id:4063,x:30340,y:33092,varname:node_4063,prsc:2|B-5564-OUT;n:type:ShaderForge.SFN_Vector4Property,id:2133,x:29928,y:33258,ptovrint:False,ptlb:MidPoint_copy,ptin:_MidPoint_copy,varname:_MidPoint_copy,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5,v2:0.5,v3:0.5,v4:1;n:type:ShaderForge.SFN_ComponentMask,id:5564,x:30125,y:33258,varname:node_5564,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2133-XYZ;n:type:ShaderForge.SFN_NormalVector,id:3595,x:32572,y:32618,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:9647,x:32799,y:32570,varname:node_9647,prsc:0,dt:1|A-9391-XYZ,B-3595-OUT;n:type:ShaderForge.SFN_Clamp01,id:4427,x:32799,y:32796,varname:node_4427,prsc:0|IN-9812-OUT;n:type:ShaderForge.SFN_Multiply,id:4504,x:32947,y:32348,varname:node_4504,prsc:0|A-9615-RGB,B-9647-OUT,C-6535-OUT;n:type:ShaderForge.SFN_Color,id:9615,x:32572,y:32232,ptovrint:False,ptlb:SunColor,ptin:_SunColor,varname:_SunColor,prsc:0,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1831,x:32984,y:32837,varname:node_1831,prsc:0|A-4427-OUT,B-8665-OUT;n:type:ShaderForge.SFN_AmbientLight,id:4441,x:32679,y:33039,varname:node_4441,prsc:2;n:type:ShaderForge.SFN_Add,id:9044,x:33498,y:32455,varname:node_9044,prsc:2|A-6981-OUT,B-1831-OUT;n:type:ShaderForge.SFN_Vector1,id:6535,x:32761,y:32405,varname:node_6535,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:8665,x:32891,y:33101,varname:node_8665,prsc:2|A-4441-RGB,B-5120-OUT;n:type:ShaderForge.SFN_Vector1,id:5120,x:32662,y:33242,varname:node_5120,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Power,id:7267,x:33137,y:32348,varname:node_7267,prsc:2|VAL-4504-OUT,EXP-6337-OUT;n:type:ShaderForge.SFN_Vector1,id:6337,x:32984,y:32682,varname:node_6337,prsc:2,v1:2;n:type:ShaderForge.SFN_Clamp01,id:6981,x:33304,y:32301,varname:node_6981,prsc:2|IN-7267-OUT;proporder:1756;pass:END;sub:END;*/

Shader "Shader Forge/metal" {
    Properties {
        _Reflection ("Reflection", 2D) = "white" {}
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Reflection; uniform float4 _Reflection_ST;
            uniform fixed4 _SunAngle;
            uniform fixed4 _SunColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                fixed3 node_4504 = (_SunColor.rgb*max(0,dot(_SunAngle.rgb,i.normalDir))*1.0);
                fixed2 node_9598 = float2(0.0,0.5*dot(i.normalDir,(viewDirection+fixed3(0,-1,0)))+0.5);
                fixed4 _Reflection_var = tex2D(_Reflection,TRANSFORM_TEX(node_9598, _Reflection));
                fixed3 node_1831 = (saturate(((0.4*pow(1.0-max(0,dot(normalDirection, viewDirection)),2.0))+_Reflection_var.r))*(UNITY_LIGHTMODEL_AMBIENT.rgb+0.1));
                float3 emissive = (saturate(pow(node_4504,2.0))+node_1831);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
