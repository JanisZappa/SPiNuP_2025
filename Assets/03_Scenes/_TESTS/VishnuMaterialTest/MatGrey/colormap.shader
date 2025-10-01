// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:1,fgcb:1,fgca:1,fgde:0.01,fgrn:109.36,fgrf:275.18,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:31208,y:31648,varname:node_3138,prsc:2|emission-472-OUT;n:type:ShaderForge.SFN_Tex2d,id:3060,x:30264,y:31730,ptovrint:False,ptlb:Map,ptin:_Map,varname:_Map,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ab3e98dec7d9cd741be1591ca1649485,ntxv:0,isnm:False|UVIN-5217-OUT;n:type:ShaderForge.SFN_ComponentMask,id:157,x:29871,y:31730,varname:node_157,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5233-XYZ;n:type:ShaderForge.SFN_RemapRange,id:5217,x:30064,y:31730,varname:node_5217,prsc:0,frmn:-1,frmx:1,tomn:0.06,tomx:0.94|IN-157-OUT;n:type:ShaderForge.SFN_Transform,id:5233,x:29686,y:31730,varname:node_5233,prsc:2,tffrom:0,tfto:3|IN-1368-OUT;n:type:ShaderForge.SFN_NormalVector,id:1368,x:29495,y:31730,prsc:2,pt:False;n:type:ShaderForge.SFN_VertexColor,id:4795,x:30264,y:31532,varname:node_4795,prsc:2;n:type:ShaderForge.SFN_Blend,id:472,x:30810,y:31653,varname:node_472,prsc:2,blmd:10,clmp:True|SRC-1145-RGB,DST-4609-OUT;n:type:ShaderForge.SFN_Multiply,id:3984,x:31029,y:31749,varname:node_3984,prsc:2|A-472-OUT,B-2224-OUT;n:type:ShaderForge.SFN_Add,id:333,x:30663,y:31863,varname:node_333,prsc:2|A-4609-OUT,B-9246-OUT;n:type:ShaderForge.SFN_Clamp01,id:2224,x:30831,y:31863,varname:node_2224,prsc:2|IN-333-OUT;n:type:ShaderForge.SFN_Vector1,id:9246,x:30415,y:31988,varname:node_9246,prsc:2,v1:0.35;n:type:ShaderForge.SFN_Desaturate,id:4609,x:30457,y:31730,varname:node_4609,prsc:0|COL-3060-RGB;n:type:ShaderForge.SFN_Color,id:1145,x:30468,y:31522,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4779412,c2:0.4779412,c3:0.4779412,c4:1;proporder:3060-1145;pass:END;sub:END;*/

Shader "Shader Forge/colorMap" {
    Properties {
        _Map ("Map", 2D) = "white" {}
        _Color ("Color", Color) = (0.4779412,0.4779412,0.4779412,1)
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
            uniform sampler2D _Map; uniform float4 _Map_ST;
            uniform fixed4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                fixed2 node_5217 = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg*0.44+0.5);
                fixed4 _Map_var = tex2D(_Map,TRANSFORM_TEX(node_5217, _Map));
                fixed node_4609 = dot(_Map_var.rgb,float3(0.3,0.59,0.11));
                float3 node_472 = saturate(( node_4609 > 0.5 ? (1.0-(1.0-2.0*(node_4609-0.5))*(1.0-_Color.rgb)) : (2.0*node_4609*_Color.rgb) ));
                float3 emissive = node_472;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
