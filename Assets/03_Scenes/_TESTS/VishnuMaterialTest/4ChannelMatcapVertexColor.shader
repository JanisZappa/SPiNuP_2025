// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:31898,y:32051,varname:node_3138,prsc:2|emission-7325-OUT;n:type:ShaderForge.SFN_Tex2d,id:3060,x:30015,y:31955,ptovrint:False,ptlb:Map,ptin:_Map,varname:_Map,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:27b21d54bb1e64645a171ac48ecdadb2,ntxv:0,isnm:False|UVIN-5217-OUT;n:type:ShaderForge.SFN_ComponentMask,id:157,x:29628,y:31955,varname:node_157,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5233-XYZ;n:type:ShaderForge.SFN_RemapRange,id:5217,x:29795,y:31955,varname:node_5217,prsc:0,frmn:-1,frmx:1,tomn:0.06,tomx:0.94|IN-157-OUT;n:type:ShaderForge.SFN_Transform,id:5233,x:29457,y:31955,varname:node_5233,prsc:2,tffrom:0,tfto:3|IN-1368-OUT;n:type:ShaderForge.SFN_NormalVector,id:1368,x:29295,y:31955,prsc:2,pt:False;n:type:ShaderForge.SFN_VertexColor,id:8750,x:30782,y:31889,varname:node_8750,prsc:2;n:type:ShaderForge.SFN_Color,id:6685,x:30015,y:32179,ptovrint:False,ptlb:Ambient,ptin:_Ambient,varname:_Ambient,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:5890,x:30431,y:32052,varname:node_5890,prsc:2|A-3060-R,B-6685-RGB;n:type:ShaderForge.SFN_Multiply,id:6567,x:30431,y:32263,varname:node_6567,prsc:2|A-3060-G,B-5390-RGB;n:type:ShaderForge.SFN_Color,id:5390,x:30015,y:32384,ptovrint:False,ptlb:Floor,ptin:_Floor,varname:_Floor,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:9524,x:30016,y:32598,ptovrint:False,ptlb:Left,ptin:_Left,varname:_Left,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1082,x:30431,y:32457,varname:node_1082,prsc:2|A-3060-B,B-9524-RGB;n:type:ShaderForge.SFN_Multiply,id:369,x:30431,y:32647,varname:node_369,prsc:2|A-3060-A,B-8564-RGB;n:type:ShaderForge.SFN_Color,id:8564,x:30016,y:32808,ptovrint:False,ptlb:Right,ptin:_Right,varname:_Right,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:6808,x:30708,y:32348,varname:node_6808,prsc:0|A-5890-OUT,B-6567-OUT,C-1082-OUT,D-369-OUT;n:type:ShaderForge.SFN_Add,id:7325,x:31424,y:32252,varname:node_7325,prsc:0|A-8750-RGB,B-7913-OUT;n:type:ShaderForge.SFN_RemapRange,id:6296,x:30902,y:32348,varname:node_6296,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-6808-OUT;n:type:ShaderForge.SFN_Add,id:7913,x:31115,y:32348,varname:node_7913,prsc:2|A-6296-OUT,B-6846-OUT;n:type:ShaderForge.SFN_Vector1,id:6846,x:30902,y:32526,varname:node_6846,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Fresnel,id:162,x:32534,y:32987,varname:node_162,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:3729,x:32866,y:33114,varname:node_3729,prsc:2|IN-4220-OUT;n:type:ShaderForge.SFN_NormalVector,id:5204,x:32372,y:33012,prsc:2,pt:False;n:type:ShaderForge.SFN_Vector3,id:2815,x:32372,y:33174,varname:node_2815,prsc:0,v1:0,v2:-1,v3:0;n:type:ShaderForge.SFN_Dot,id:2770,x:32534,y:33114,varname:node_2770,prsc:2,dt:0|A-5204-OUT,B-2815-OUT;n:type:ShaderForge.SFN_Multiply,id:4220,x:32695,y:33114,varname:node_4220,prsc:2|A-162-OUT,B-2770-OUT;n:type:ShaderForge.SFN_Fresnel,id:3414,x:32598,y:33051,varname:node_3414,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:2025,x:32930,y:33178,varname:node_2025,prsc:2|IN-7666-OUT;n:type:ShaderForge.SFN_NormalVector,id:8242,x:32436,y:33076,prsc:2,pt:False;n:type:ShaderForge.SFN_Vector3,id:8011,x:32436,y:33238,varname:node_8011,prsc:0,v1:0,v2:-1,v3:0;n:type:ShaderForge.SFN_Dot,id:7782,x:32598,y:33178,varname:node_7782,prsc:2,dt:0|A-8242-OUT,B-8011-OUT;n:type:ShaderForge.SFN_Multiply,id:7666,x:32759,y:33178,varname:node_7666,prsc:2|A-3414-OUT,B-7782-OUT;proporder:3060-6685-5390-9524-8564;pass:END;sub:END;*/

Shader "andy/4ChannelMatcapVertexColor" {
    Properties {
        _Map ("Map", 2D) = "white" {}
        _Ambient ("Ambient", Color) = (1,1,1,1)
        _Floor ("Floor", Color) = (1,1,1,1)
        _Left ("Left", Color) = (1,1,1,1)
        _Right ("Right", Color) = (1,1,1,1)
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
            uniform fixed4 _Ambient;
            uniform fixed4 _Floor;
            uniform fixed4 _Left;
            uniform fixed4 _Right;
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
                float3 emissive = (i.vertexColor.rgb+((((_Map_var.r*_Ambient.rgb)+(_Map_var.g*_Floor.rgb)+(_Map_var.b*_Left.rgb)+(_Map_var.a*_Right.rgb))*2.0+-1.0)+0.5));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
