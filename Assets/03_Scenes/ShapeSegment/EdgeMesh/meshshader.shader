// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33765,y:32779,varname:node_4013,prsc:2|diff-374-OUT,spec-6453-OUT,gloss-4558-OUT,emission-9319-OUT;n:type:ShaderForge.SFN_VertexColor,id:8353,x:31998,y:32761,varname:node_8353,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2609,x:32198,y:32815,varname:node_2609,prsc:2|A-8353-R,B-6357-OUT;n:type:ShaderForge.SFN_Vector1,id:6357,x:31998,y:32891,varname:node_6357,prsc:2,v1:255;n:type:ShaderForge.SFN_Step,id:2695,x:32403,y:32873,varname:node_2695,prsc:2|A-2609-OUT,B-170-OUT;n:type:ShaderForge.SFN_Vector1,id:170,x:32231,y:33005,varname:node_170,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Lerp,id:374,x:32726,y:32715,varname:node_374,prsc:2|A-4755-RGB,B-9142-RGB,T-2695-OUT;n:type:ShaderForge.SFN_Color,id:9142,x:32434,y:32731,ptovrint:False,ptlb:ColorB,ptin:_ColorB,varname:node_9142,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4755,x:32434,y:32541,ptovrint:False,ptlb:Tex,ptin:_Tex,varname:node_4755,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3649-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:5919,x:32046,y:32522,varname:node_5919,prsc:2;n:type:ShaderForge.SFN_Append,id:3649,x:32253,y:32541,varname:node_3649,prsc:2|A-5919-X,B-5919-Y;n:type:ShaderForge.SFN_Vector1,id:6453,x:32726,y:32849,varname:node_6453,prsc:2,v1:0.05;n:type:ShaderForge.SFN_LightVector,id:387,x:32366,y:33125,varname:node_387,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7440,x:32541,y:33174,varname:node_7440,prsc:2|A-387-OUT,B-3931-OUT;n:type:ShaderForge.SFN_Vector1,id:3931,x:32366,y:33258,varname:node_3931,prsc:2,v1:-1;n:type:ShaderForge.SFN_NormalVector,id:5934,x:32541,y:33308,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:8519,x:32730,y:33228,varname:node_8519,prsc:2,dt:4|A-7440-OUT,B-5934-OUT;n:type:ShaderForge.SFN_Multiply,id:714,x:32965,y:33228,varname:node_714,prsc:2|A-8519-OUT,B-5011-OUT;n:type:ShaderForge.SFN_Vector1,id:508,x:33150,y:33353,varname:node_508,prsc:2,v1:0.175;n:type:ShaderForge.SFN_LightColor,id:7244,x:32565,y:33494,varname:node_7244,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:4646,x:32736,y:33494,varname:node_4646,prsc:2|IN-7244-RGB;n:type:ShaderForge.SFN_Vector1,id:1409,x:32736,y:33623,varname:node_1409,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:5011,x:32952,y:33494,varname:node_5011,prsc:2|A-4646-OUT,B-1409-OUT,T-1458-OUT;n:type:ShaderForge.SFN_Vector1,id:1458,x:32952,y:33623,varname:node_1458,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Clamp01,id:5738,x:33133,y:33228,varname:node_5738,prsc:2|IN-714-OUT;n:type:ShaderForge.SFN_Multiply,id:5807,x:33318,y:33168,varname:node_5807,prsc:2|A-5738-OUT,B-508-OUT;n:type:ShaderForge.SFN_Vector1,id:4558,x:33158,y:32902,varname:node_4558,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:9319,x:33518,y:33168,varname:node_9319,prsc:2|A-5807-OUT,B-7914-OUT;n:type:ShaderForge.SFN_Fresnel,id:2039,x:33290,y:33434,varname:node_2039,prsc:2;n:type:ShaderForge.SFN_Power,id:5293,x:33466,y:33434,varname:node_5293,prsc:2|VAL-2039-OUT,EXP-1198-OUT;n:type:ShaderForge.SFN_Vector1,id:1198,x:33290,y:33572,varname:node_1198,prsc:2,v1:10;n:type:ShaderForge.SFN_Clamp01,id:6638,x:33652,y:33434,varname:node_6638,prsc:2|IN-5293-OUT;n:type:ShaderForge.SFN_Multiply,id:7914,x:33827,y:33434,varname:node_7914,prsc:2|A-6638-OUT,B-5350-OUT;n:type:ShaderForge.SFN_Vector1,id:5350,x:33652,y:33563,varname:node_5350,prsc:2,v1:0.125;proporder:9142-4755;pass:END;sub:END;*/

Shader "Shader Forge/meshshader" {
    Properties {
        _ColorB ("ColorB", Color) = (0.5,0.5,0.5,1)
        _Tex ("Tex", 2D) = "white" {}
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
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _ColorB;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
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
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 0.0;
                float perceptualRoughness = 1.0 - 0.0;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.05;
                float specularMonochrome;
                float2 node_3649 = float2(i.posWorld.r,i.posWorld.g);
                float4 _Tex_var = tex2D(_Tex,TRANSFORM_TEX(node_3649, _Tex));
                float3 node_374 = lerp(_Tex_var.rgb,_ColorB.rgb,step((i.vertexColor.r*255.0),0.5));
                float3 diffuseColor = node_374; // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float node_1409 = 1.0;
                float3 emissive = ((saturate((0.5*dot((lightDirection*(-1.0)),i.normalDir)+0.5*lerp((1.0 - _LightColor0.rgb),float3(node_1409,node_1409,node_1409),0.25)))*0.175)+(saturate(pow((1.0-max(0,dot(normalDirection, viewDirection))),10.0))*0.125));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _ColorB;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
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
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 0.0;
                float perceptualRoughness = 1.0 - 0.0;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.05;
                float specularMonochrome;
                float2 node_3649 = float2(i.posWorld.r,i.posWorld.g);
                float4 _Tex_var = tex2D(_Tex,TRANSFORM_TEX(node_3649, _Tex));
                float3 node_374 = lerp(_Tex_var.rgb,_ColorB.rgb,step((i.vertexColor.r*255.0),0.5));
                float3 diffuseColor = node_374; // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
