// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33235,y:32153,varname:node_3138,prsc:2|emission-8877-OUT;n:type:ShaderForge.SFN_Tex2d,id:3060,x:30015,y:31955,ptovrint:False,ptlb:Map,ptin:_Map,varname:_Map,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:27b21d54bb1e64645a171ac48ecdadb2,ntxv:0,isnm:False|UVIN-5217-OUT;n:type:ShaderForge.SFN_ComponentMask,id:157,x:29628,y:31955,varname:node_157,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5233-XYZ;n:type:ShaderForge.SFN_RemapRange,id:5217,x:29795,y:31955,varname:node_5217,prsc:0,frmn:-1,frmx:1,tomn:0.06,tomx:0.94|IN-157-OUT;n:type:ShaderForge.SFN_Transform,id:5233,x:29457,y:31955,varname:node_5233,prsc:2,tffrom:0,tfto:3|IN-1368-OUT;n:type:ShaderForge.SFN_NormalVector,id:1368,x:29295,y:31955,prsc:2,pt:False;n:type:ShaderForge.SFN_VertexColor,id:8750,x:30868,y:31779,varname:node_8750,prsc:2;n:type:ShaderForge.SFN_Power,id:4866,x:31104,y:31880,varname:node_4866,prsc:0|VAL-8750-RGB,EXP-4244-OUT;n:type:ShaderForge.SFN_Vector1,id:4244,x:30883,y:31961,varname:node_4244,prsc:2,v1:0.33;n:type:ShaderForge.SFN_Clamp01,id:6800,x:31337,y:31880,varname:node_6800,prsc:0|IN-4866-OUT;n:type:ShaderForge.SFN_Color,id:6685,x:30015,y:32179,ptovrint:False,ptlb:Ambient,ptin:_Ambient,varname:_Ambient,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:5890,x:31005,y:32112,varname:node_5890,prsc:2|A-3060-R,B-6685-RGB,C-6800-OUT,D-6874-OUT;n:type:ShaderForge.SFN_Multiply,id:6567,x:31005,y:32299,varname:node_6567,prsc:2|A-3060-G,B-5390-RGB,C-6800-OUT,D-6874-OUT;n:type:ShaderForge.SFN_Color,id:5390,x:30015,y:32384,ptovrint:False,ptlb:Floor,ptin:_Floor,varname:_Floor,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:9524,x:30016,y:32598,ptovrint:False,ptlb:Left,ptin:_Left,varname:_Left,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1082,x:31005,y:32513,varname:node_1082,prsc:2|A-3060-B,B-9524-RGB,C-6800-OUT,D-6874-OUT;n:type:ShaderForge.SFN_Multiply,id:369,x:31005,y:32703,varname:node_369,prsc:2|A-3060-A,B-8564-RGB,C-6800-OUT,D-6874-OUT;n:type:ShaderForge.SFN_Color,id:8564,x:30016,y:32809,ptovrint:False,ptlb:Right,ptin:_Right,varname:_Right,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:6808,x:31282,y:32408,varname:node_6808,prsc:0|A-5890-OUT,B-6567-OUT,C-1082-OUT,D-369-OUT;n:type:ShaderForge.SFN_Color,id:6235,x:31368,y:32157,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Add,id:7325,x:32479,y:32429,varname:node_7325,prsc:0|A-3215-OUT,B-68-OUT,C-8004-OUT;n:type:ShaderForge.SFN_Desaturate,id:6222,x:31530,y:32408,varname:node_6222,prsc:0|COL-6808-OUT;n:type:ShaderForge.SFN_Multiply,id:3215,x:31808,y:32384,varname:node_3215,prsc:0|A-6235-RGB,B-6222-OUT;n:type:ShaderForge.SFN_Vector1,id:6874,x:30466,y:32930,varname:node_6874,prsc:0,v1:1.25;n:type:ShaderForge.SFN_Multiply,id:68,x:31675,y:32760,varname:node_68,prsc:2|A-6808-OUT,B-1165-OUT;n:type:ShaderForge.SFN_Vector1,id:1165,x:31476,y:32822,varname:node_1165,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Multiply,id:8004,x:31820,y:32565,varname:node_8004,prsc:2|A-6222-OUT,B-4919-OUT;n:type:ShaderForge.SFN_Vector1,id:4919,x:31638,y:32664,varname:node_4919,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Lerp,id:8466,x:32668,y:32226,varname:node_8466,prsc:2|A-6235-RGB,B-7325-OUT,T-7155-OUT;n:type:ShaderForge.SFN_Vector1,id:7155,x:32197,y:32385,varname:node_7155,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Multiply,id:8877,x:33006,y:32203,varname:node_8877,prsc:2|A-9717-OUT,B-8466-OUT;n:type:ShaderForge.SFN_Add,id:320,x:31477,y:31682,varname:node_320,prsc:2|A-6840-OUT,B-4866-OUT;n:type:ShaderForge.SFN_Vector1,id:6840,x:31286,y:31584,varname:node_6840,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Clamp01,id:9717,x:31962,y:31750,varname:node_9717,prsc:2|IN-320-OUT;proporder:3060-6685-5390-9524-8564-6235;pass:END;sub:END;*/

Shader "andy/4CMBetter" {
    Properties {
        _Map ("Map", 2D) = "white" {}
        _Ambient ("Ambient", Color) = (1,1,1,1)
        _Floor ("Floor", Color) = (1,1,1,1)
        _Left ("Left", Color) = (1,1,1,1)
        _Right ("Right", Color) = (1,1,1,1)
        _Color ("Color", Color) = (0,0,0,1)
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
            uniform fixed4 _Color;
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
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                fixed3 node_4866 = pow(i.vertexColor.rgb,0.33);
                fixed2 node_5217 = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg*0.44+0.5);
                fixed4 _Map_var = tex2D(_Map,TRANSFORM_TEX(node_5217, _Map));
                fixed3 node_6800 = saturate(node_4866);
                fixed node_6874 = 1.25;
                fixed3 node_6808 = ((_Map_var.r*_Ambient.rgb*node_6800*node_6874)+(_Map_var.g*_Floor.rgb*node_6800*node_6874)+(_Map_var.b*_Left.rgb*node_6800*node_6874)+(_Map_var.a*_Right.rgb*node_6800*node_6874));
                fixed node_6222 = dot(node_6808,float3(0.3,0.59,0.11));
                float3 emissive = (saturate((0.1+node_4866))*lerp(_Color.rgb,((_Color.rgb*node_6222)+(node_6808*0.3)+(node_6222*0.5)),0.75));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
