// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32024,y:31733,varname:node_3138,prsc:2|emission-5985-OUT;n:type:ShaderForge.SFN_Tex2d,id:3060,x:30015,y:31955,ptovrint:False,ptlb:Diff,ptin:_Diff,varname:_Diff,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:27b21d54bb1e64645a171ac48ecdadb2,ntxv:0,isnm:False|UVIN-5217-OUT;n:type:ShaderForge.SFN_ComponentMask,id:157,x:29628,y:31955,varname:node_157,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-5233-XYZ;n:type:ShaderForge.SFN_RemapRange,id:5217,x:29795,y:31955,varname:node_5217,prsc:0,frmn:-1,frmx:1,tomn:0.06,tomx:0.94|IN-157-OUT;n:type:ShaderForge.SFN_Transform,id:5233,x:29457,y:31955,varname:node_5233,prsc:2,tffrom:0,tfto:3|IN-1368-OUT;n:type:ShaderForge.SFN_NormalVector,id:1368,x:29295,y:31955,prsc:2,pt:False;n:type:ShaderForge.SFN_VertexColor,id:8750,x:30071,y:31686,varname:node_8750,prsc:2;n:type:ShaderForge.SFN_Power,id:4866,x:30562,y:31820,varname:node_4866,prsc:0|VAL-8750-RGB,EXP-4244-OUT;n:type:ShaderForge.SFN_Vector1,id:4244,x:30309,y:31901,varname:node_4244,prsc:2,v1:0.33;n:type:ShaderForge.SFN_Clamp01,id:6800,x:30763,y:31820,varname:node_6800,prsc:0|IN-4866-OUT;n:type:ShaderForge.SFN_Color,id:6685,x:30015,y:32179,ptovrint:False,ptlb:Ambient,ptin:_Ambient,varname:_Ambient,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:5890,x:30431,y:32052,varname:node_5890,prsc:2|A-3060-R,B-6685-RGB;n:type:ShaderForge.SFN_Multiply,id:6567,x:30431,y:32174,varname:node_6567,prsc:2|A-3060-G,B-5390-RGB;n:type:ShaderForge.SFN_Color,id:5390,x:30015,y:32384,ptovrint:False,ptlb:Floor,ptin:_Floor,varname:_Floor,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:9524,x:30016,y:32598,ptovrint:False,ptlb:Left,ptin:_Left,varname:_Left,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1082,x:30431,y:32295,varname:node_1082,prsc:2|A-3060-B,B-9524-RGB;n:type:ShaderForge.SFN_Multiply,id:369,x:30431,y:32412,varname:node_369,prsc:2|A-3060-A,B-8564-RGB;n:type:ShaderForge.SFN_Color,id:8564,x:30016,y:32808,ptovrint:False,ptlb:Right,ptin:_Right,varname:_Right,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:6808,x:30648,y:32196,varname:node_6808,prsc:0|A-5890-OUT,B-6567-OUT,C-1082-OUT,D-369-OUT;n:type:ShaderForge.SFN_Color,id:6235,x:31038,y:32304,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_RemapRange,id:5103,x:30340,y:31554,varname:node_5103,prsc:2,frmn:0,frmx:1.5,tomn:-0.1,tomx:1|IN-8750-RGB;n:type:ShaderForge.SFN_Add,id:4475,x:30544,y:31538,varname:node_4475,prsc:2|A-377-OUT,B-5103-OUT;n:type:ShaderForge.SFN_Vector1,id:377,x:30340,y:31475,varname:node_377,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:3613,x:31175,y:32107,varname:node_3613,prsc:2|A-808-OUT,B-6235-RGB;n:type:ShaderForge.SFN_Tex2d,id:5663,x:30014,y:33131,ptovrint:False,ptlb:Ref,ptin:_Ref,varname:_Ref,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5217-OUT;n:type:ShaderForge.SFN_Multiply,id:4333,x:30479,y:32974,varname:node_4333,prsc:2|A-6685-RGB,B-5663-R;n:type:ShaderForge.SFN_Multiply,id:6046,x:30479,y:33107,varname:node_6046,prsc:2|A-5390-RGB,B-5663-G;n:type:ShaderForge.SFN_Multiply,id:3623,x:30484,y:33230,varname:node_3623,prsc:2|A-9524-RGB,B-5663-B;n:type:ShaderForge.SFN_Multiply,id:2732,x:30490,y:33354,varname:node_2732,prsc:2|A-8564-RGB,B-5663-A;n:type:ShaderForge.SFN_Multiply,id:808,x:30850,y:32152,varname:node_808,prsc:2|A-6800-OUT,B-6808-OUT;n:type:ShaderForge.SFN_Add,id:2114,x:30713,y:33175,varname:node_2114,prsc:2|A-4333-OUT,B-6046-OUT,C-3623-OUT,D-2732-OUT;n:type:ShaderForge.SFN_Multiply,id:1356,x:31168,y:33257,varname:node_1356,prsc:2|A-1869-OUT,B-2114-OUT;n:type:ShaderForge.SFN_Add,id:5985,x:31574,y:32236,varname:node_5985,prsc:2|A-3613-OUT,B-1356-OUT;n:type:ShaderForge.SFN_Desaturate,id:3665,x:31221,y:32591,varname:node_3665,prsc:2|COL-6235-RGB;n:type:ShaderForge.SFN_Lerp,id:1869,x:31547,y:32901,varname:node_1869,prsc:2|A-313-OUT,B-6235-RGB,T-3665-OUT;n:type:ShaderForge.SFN_Vector1,id:313,x:31239,y:32931,varname:node_313,prsc:2,v1:1;proporder:3060-6685-5390-9524-8564-6235-5663;pass:END;sub:END;*/

Shader "andy/4Chan" {
    Properties {
        _Diff ("Diff", 2D) = "white" {}
        _Ambient ("Ambient", Color) = (1,1,1,1)
        _Floor ("Floor", Color) = (1,1,1,1)
        _Left ("Left", Color) = (1,1,1,1)
        _Right ("Right", Color) = (1,1,1,1)
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Ref ("Ref", 2D) = "white" {}
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
            uniform sampler2D _Diff; uniform float4 _Diff_ST;
            uniform fixed4 _Ambient;
            uniform fixed4 _Floor;
            uniform fixed4 _Left;
            uniform fixed4 _Right;
            uniform fixed4 _Color;
            uniform sampler2D _Ref; uniform float4 _Ref_ST;
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
                fixed4 _Diff_var = tex2D(_Diff,TRANSFORM_TEX(node_5217, _Diff));
                float node_313 = 1.0;
                fixed4 _Ref_var = tex2D(_Ref,TRANSFORM_TEX(node_5217, _Ref));
                float3 emissive = (((saturate(pow(i.vertexColor.rgb,0.33))*((_Diff_var.r*_Ambient.rgb)+(_Diff_var.g*_Floor.rgb)+(_Diff_var.b*_Left.rgb)+(_Diff_var.a*_Right.rgb)))*_Color.rgb)+(lerp(float3(node_313,node_313,node_313),_Color.rgb,dot(_Color.rgb,float3(0.3,0.59,0.11)))*((_Ambient.rgb*_Ref_var.r)+(_Floor.rgb*_Ref_var.g)+(_Left.rgb*_Ref_var.b)+(_Right.rgb*_Ref_var.a))));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
