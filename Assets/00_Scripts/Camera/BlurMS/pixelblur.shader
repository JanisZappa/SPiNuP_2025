// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:1,fgcb:1,fgca:1,fgde:0.01,fgrn:109.36,fgrf:275.18,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33633,y:32370,varname:node_3138,prsc:2|emission-7387-OUT;n:type:ShaderForge.SFN_Tex2d,id:4312,x:32325,y:32397,varname:Derp,prsc:2,ntxv:0,isnm:False|UVIN-8925-OUT,TEX-7359-TEX;n:type:ShaderForge.SFN_Tex2d,id:6289,x:31212,y:32350,varname:node_6289,prsc:2,ntxv:0,isnm:False|UVIN-1875-OUT,TEX-7794-TEX;n:type:ShaderForge.SFN_RemapRange,id:2129,x:31415,y:32366,varname:node_2129,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-6289-R;n:type:ShaderForge.SFN_Tex2dAsset,id:7359,x:32296,y:31875,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:406,x:29546,y:32280,ptovrint:False,ptlb:Width,ptin:_Width,varname:_Width,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:5291,x:29546,y:32409,ptovrint:False,ptlb:Height,ptin:_Height,varname:_Height,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:7793,x:29730,y:32136,varname:node_7793,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:5046,x:32584,y:31840,varname:_node_5046,prsc:2,ntxv:0,isnm:False|TEX-7359-TEX;n:type:ShaderForge.SFN_Lerp,id:4015,x:32805,y:32400,varname:node_4015,prsc:2|A-5046-RGB,B-4312-RGB,T-5101-OUT;n:type:ShaderForge.SFN_Vector1,id:5101,x:32621,y:32643,varname:node_5101,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Tex2dAsset,id:7794,x:30082,y:32748,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:_Noise,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:4617,x:31617,y:32354,varname:node_4617,prsc:2|A-2129-OUT,B-2129-OUT;n:type:ShaderForge.SFN_Append,id:9107,x:29730,y:32322,varname:node_9107,prsc:2|A-406-OUT,B-5291-OUT;n:type:ShaderForge.SFN_Multiply,id:8052,x:29939,y:32136,varname:node_8052,prsc:2|A-7793-UVOUT,B-9107-OUT;n:type:ShaderForge.SFN_Floor,id:1901,x:30110,y:32136,varname:node_1901,prsc:2|IN-8052-OUT;n:type:ShaderForge.SFN_Multiply,id:1875,x:30911,y:32343,varname:node_1875,prsc:2|A-9107-OUT,B-3835-UVOUT,C-1088-OUT;n:type:ShaderForge.SFN_TexCoord,id:3835,x:30590,y:32389,varname:node_3835,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:1088,x:30911,y:32521,varname:node_1088,prsc:2,v1:0.015625;n:type:ShaderForge.SFN_Add,id:5883,x:31797,y:32216,varname:node_5883,prsc:2|A-1901-OUT,B-4617-OUT;n:type:ShaderForge.SFN_Divide,id:8925,x:32057,y:32398,varname:node_8925,prsc:2|A-5883-OUT,B-9107-OUT;n:type:ShaderForge.SFN_Tex2d,id:1841,x:32337,y:32922,varname:_node_2699,prsc:2,ntxv:0,isnm:False|UVIN-9215-OUT,TEX-7359-TEX;n:type:ShaderForge.SFN_Tex2d,id:6475,x:31224,y:32875,varname:_node_2142,prsc:2,ntxv:0,isnm:False|UVIN-8744-OUT,TEX-7794-TEX;n:type:ShaderForge.SFN_RemapRange,id:7146,x:31427,y:32891,varname:node_7146,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-6475-R;n:type:ShaderForge.SFN_Append,id:1887,x:31629,y:32879,varname:node_1887,prsc:2|A-7146-OUT,B-7146-OUT;n:type:ShaderForge.SFN_Multiply,id:8744,x:30923,y:32868,varname:node_8744,prsc:2|A-9107-OUT,B-4121-OUT,C-5694-OUT;n:type:ShaderForge.SFN_TexCoord,id:9877,x:30485,y:32880,varname:node_9877,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:5694,x:30923,y:33046,varname:node_5694,prsc:2,v1:0.015625;n:type:ShaderForge.SFN_Add,id:6632,x:31809,y:32741,varname:node_6632,prsc:2|A-1901-OUT,B-1887-OUT;n:type:ShaderForge.SFN_Divide,id:9215,x:32069,y:32923,varname:node_9215,prsc:2|A-6632-OUT,B-9107-OUT;n:type:ShaderForge.SFN_Lerp,id:7387,x:33171,y:32713,varname:node_7387,prsc:2|A-4015-OUT,B-1841-RGB,T-5101-OUT;n:type:ShaderForge.SFN_Add,id:4121,x:30686,y:32946,varname:node_4121,prsc:2|A-9877-UVOUT,B-6186-OUT;n:type:ShaderForge.SFN_Vector2,id:6186,x:30464,y:33241,varname:node_6186,prsc:2,v1:0.5,v2:0.25;proporder:7359-406-5291-7794;pass:END;sub:END;*/

Shader "andy/pixelblur" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Width ("Width", Float ) = 0
        _Height ("Height", Float ) = 0
        _Noise ("Noise", 2D) = "white" {}
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
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Width;
            uniform float _Height;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _node_5046 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float2 node_9107 = float2(_Width,_Height);
                float2 node_1901 = floor((i.uv0*node_9107));
                float2 node_1875 = (node_9107*i.uv0*0.015625);
                float4 node_6289 = tex2D(_Noise,TRANSFORM_TEX(node_1875, _Noise));
                float node_2129 = (node_6289.r*2.0+-1.0);
                float2 node_8925 = ((node_1901+float2(node_2129,node_2129))/node_9107);
                float4 Derp = tex2D(_MainTex,TRANSFORM_TEX(node_8925, _MainTex));
                float node_5101 = 0.5;
                float2 node_8744 = (node_9107*(i.uv0+float2(0.5,0.25))*0.015625);
                float4 _node_2142 = tex2D(_Noise,TRANSFORM_TEX(node_8744, _Noise));
                float node_7146 = (_node_2142.r*1.0+-0.5);
                float2 node_9215 = ((node_1901+float2(node_7146,node_7146))/node_9107);
                float4 _node_2699 = tex2D(_MainTex,TRANSFORM_TEX(node_9215, _MainTex));
                float3 emissive = lerp(lerp(_node_5046.rgb,Derp.rgb,node_5101),_node_2699.rgb,node_5101);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
