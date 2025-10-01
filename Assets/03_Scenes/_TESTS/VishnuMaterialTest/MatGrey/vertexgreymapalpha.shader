// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:1,fgcb:1,fgca:1,fgde:0.01,fgrn:109.36,fgrf:275.18,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:31382,y:31668,varname:node_3138,prsc:2|emission-3984-OUT,alpha-666-OUT;n:type:ShaderForge.SFN_Tex2d,id:3060,x:30232,y:31773,ptovrint:False,ptlb:Map,ptin:_Map,varname:_Map,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ab3e98dec7d9cd741be1591ca1649485,ntxv:0,isnm:False|UVIN-5217-OUT;n:type:ShaderForge.SFN_ComponentMask,id:157,x:29831,y:31773,varname:node_157,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5233-XYZ;n:type:ShaderForge.SFN_RemapRange,id:5217,x:30034,y:31773,varname:node_5217,prsc:0,frmn:-1,frmx:1,tomn:0.06,tomx:0.94|IN-157-OUT;n:type:ShaderForge.SFN_Transform,id:5233,x:29654,y:31773,varname:node_5233,prsc:2,tffrom:0,tfto:3|IN-1368-OUT;n:type:ShaderForge.SFN_NormalVector,id:1368,x:29471,y:31773,prsc:2,pt:False;n:type:ShaderForge.SFN_VertexColor,id:4795,x:30550,y:31597,varname:node_4795,prsc:2;n:type:ShaderForge.SFN_Blend,id:472,x:30810,y:31653,varname:node_472,prsc:2,blmd:10,clmp:True|SRC-4795-RGB,DST-3060-RGB;n:type:ShaderForge.SFN_Multiply,id:3984,x:31013,y:31867,varname:node_3984,prsc:2|A-472-OUT,B-2224-OUT;n:type:ShaderForge.SFN_Add,id:333,x:30663,y:31863,varname:node_333,prsc:2|A-3060-RGB,B-9246-OUT;n:type:ShaderForge.SFN_Clamp01,id:2224,x:30807,y:31949,varname:node_2224,prsc:2|IN-333-OUT;n:type:ShaderForge.SFN_Vector1,id:9246,x:30459,y:32064,varname:node_9246,prsc:2,v1:0.35;n:type:ShaderForge.SFN_Vector1,id:666,x:31002,y:32289,varname:node_666,prsc:2,v1:0.92;proporder:3060;pass:END;sub:END;*/

Shader "Shader Forge/vertecolorgreymapalpha" {
    Properties {
        _Map ("Map", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 2.0
            uniform sampler2D _Map; uniform float4 _Map_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                fixed2 node_5217 = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg*0.44+0.5);
                fixed4 _Map_var = tex2D(_Map,TRANSFORM_TEX(node_5217, _Map));
                float3 emissive = (saturate(( _Map_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_Map_var.rgb-0.5))*(1.0-i.vertexColor.rgb)) : (2.0*_Map_var.rgb*i.vertexColor.rgb) ))*saturate((_Map_var.rgb+0.35)));
                float3 finalColor = emissive;
                return fixed4(finalColor,0.92);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
