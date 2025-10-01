// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33340,y:32920,varname:node_4013,prsc:2|emission-143-OUT;n:type:ShaderForge.SFN_NormalVector,id:9051,x:31981,y:33109,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:795,x:32166,y:33058,varname:node_795,prsc:0,dt:4|A-5320-OUT,B-9051-OUT;n:type:ShaderForge.SFN_VertexColor,id:9681,x:31442,y:32365,varname:node_9681,prsc:2;n:type:ShaderForge.SFN_Min,id:9280,x:32447,y:33143,varname:node_9280,prsc:0|A-795-OUT,B-9681-A;n:type:ShaderForge.SFN_LightVector,id:1842,x:32202,y:33423,varname:node_1842,prsc:2;n:type:ShaderForge.SFN_Dot,id:980,x:32394,y:33479,varname:node_980,prsc:2,dt:1|A-5320-OUT,B-555-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:555,x:32202,y:33559,varname:node_555,prsc:2;n:type:ShaderForge.SFN_Power,id:8057,x:32593,y:33479,varname:node_8057,prsc:2|VAL-980-OUT,EXP-6086-OUT;n:type:ShaderForge.SFN_Vector1,id:6086,x:32394,y:33631,varname:node_6086,prsc:2,v1:15;n:type:ShaderForge.SFN_Multiply,id:3068,x:32781,y:33151,varname:node_3068,prsc:2|A-9280-OUT,B-3894-OUT,C-2582-OUT;n:type:ShaderForge.SFN_Clamp01,id:3894,x:32809,y:33479,varname:node_3894,prsc:2|IN-8057-OUT;n:type:ShaderForge.SFN_Add,id:143,x:32857,y:32750,varname:node_143,prsc:2|A-2704-OUT,B-3068-OUT;n:type:ShaderForge.SFN_Vector1,id:2582,x:32621,y:33323,varname:node_2582,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:4112,x:32188,y:32383,varname:node_4112,prsc:2|A-9681-RGB,B-179-OUT;n:type:ShaderForge.SFN_Multiply,id:2704,x:32565,y:32651,varname:node_2704,prsc:2|A-4112-OUT,B-1073-OUT;n:type:ShaderForge.SFN_Color,id:1690,x:31441,y:32641,ptovrint:False,ptlb:TintColor,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:179,x:31811,y:32440,varname:node_179,prsc:2|A-1690-RGB,B-3363-OUT,T-9681-A;n:type:ShaderForge.SFN_Vector1,id:3363,x:31616,y:32702,varname:node_3363,prsc:2,v1:1;n:type:ShaderForge.SFN_Clamp01,id:1073,x:32282,y:32722,varname:node_1073,prsc:2|IN-795-OUT;n:type:ShaderForge.SFN_Vector3,id:8526,x:31458,y:32953,varname:node_8526,prsc:2,v1:1,v2:1,v3:0.2;n:type:ShaderForge.SFN_Normalize,id:5320,x:31637,y:33418,varname:node_5320,prsc:2|IN-8526-OUT;proporder:1690;pass:END;sub:END;*/

Shader "Shader Forge/tracktest" {
    Properties {
        _TintColor ("TintColor", Color) = (0.5,0.5,0.5,1)
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _TintColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float node_3363 = 1.0;
                float3 node_5320 = normalize(float3(1,1,0.2));
                fixed node_795 = 0.5*dot(node_5320,i.normalDir)+0.5;
                float3 emissive = (((i.vertexColor.rgb*lerp(_TintColor.rgb,float3(node_3363,node_3363,node_3363),i.vertexColor.a))*saturate(node_795))+(min(node_795,i.vertexColor.a)*saturate(pow(max(0,dot(node_5320,viewReflectDirection)),15.0))*1.0));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
