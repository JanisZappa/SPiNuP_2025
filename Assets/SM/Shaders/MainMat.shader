Shader "andy/MainMat" 
{
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.5

            #pragma multi_compile NORMALS_OFF    NORMALS_ON
            #pragma multi_compile GREY_OFF       GREY_ON
            #pragma multi_compile DESATURATE_OFF DESATURATE_ON
            #pragma multi_compile TEST_OFF       TEST_ON
            
            #include "UnityCG.cginc"
            #include "Palette.cginc"
            

        //  PreCalc | Side * .5 + .5  //
            uniform fixed SideMulti;
            
        //  PreCalc | .65 + saturate(-Side) * .35  //
            uniform fixed WallCastSideMulti;
            
        //  PreCalc | 1  + saturate(Side) * .1  //
            uniform fixed MegaSideMulti;
            
        //  PreCalc | .125 - saturate(Side) * .025  //
            uniform fixed ContactAdd;

            uniform fixed3 AntiSunColor;
            uniform fixed3 Ambient;
            uniform fixed3 ShadowColor;
            uniform fixed  CastShadowVis;

            
            

            struct VertexInput 
            {
                float4 vertex      : POSITION;
                float3 normal      : NORMAL;
                float4 vertexColor : COLOR;
            };


            struct VertexOutput
            {
                float4 vertexColor : COLOR;

                fixed4 matCapWall : TEXCOORD1;
                fixed4 mapUVs     : TEXCOORD2;
                fixed4 masks      : TEXCOORD3;
                fixed4 moreMasks  : TEXCOORD6;
                fixed4 fog        : TEXCOORD0;
                
                fixed4 texCoord4 : TEXCOORD4;
                half4  texCoord5 : TEXCOORD5;
            };




            VertexOutput vert (VertexInput v, out float4 outpos : SV_POSITION) 
            {
                VertexOutput o = (VertexOutput)0;
                outpos = UnityObjectToClipPos(v.vertex);

            //  Show Normals  //
                #if NORMALS_ON
                    o.vertexColor = fixed4(UnityObjectToWorldNormal(v.normal), 1);
                    return o;
                #endif


            //  Default  //
                float4 color    = v.vertexColor;
                float3 normal   = UnityObjectToWorldNormal(v.normal);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                
            //  Shadow Masks  //
                Masks masks = GetMasks(color, saturate(sign(worldPos.z * Side)));


            //  Dot Products  //
                fixed sunLight    = saturate(dot(SunAngle, normal)) * (1 - HeightLightAdd) + HeightLightAdd;
                fixed bottomLight = 1 + (1 - saturate(normal.y * .5 + 1.25)) * BottomLightBounceMulti;
                fixed noShadows   = saturate(dot(float3(SunAngle.xy, SunAngle.z * -Side), normal) * -1.5 +  .05);


            //  WallStuff  //
                fixed wallLerp   = (Wall - worldPos.z) / (Wall * 2);
                fixed shadeMulti = (.35 + .65 * sunLight) * .36;
                fixed absPosZ    = abs(worldPos.z);
                fixed wallShade  = (1 + ((ContactAdd + (absPosZ - Wall) * shadeMulti) + abs(normal.z) * .075 - 1) * (1 - masks.contact)) * 1.15;
                
                fixed bodyPow = GamePlane - absPosZ;
              

            //  Result  //
                float2 cmID = CM_ID(color);
                
                o.vertexColor = fixed4(GetColor(cmID.x), color.a);
                o.matCapWall  = fixed4(GetMatCapUV(cmID.y, normal), wallShade, color.b);
                o.mapUVs      = GetMapUVs(worldPos);
                o.masks       = masks.Get();
                o.moreMasks   = masks.GetMore();
                o.fog         = fixed4(GetFogUV(worldPos), SideMulti - wallLerp * Side);
                
                o.texCoord4 = fixed4(sunLight, bodyPow, bottomLight, noShadows);
                o.texCoord5 = half4(1 - wallLerp, worldPos.xyz - _WorldSpaceCameraPos);
  
                return o;
            }


            float4 frag(VertexOutput i, UNITY_VPOS_TYPE vpos : VPOS) : COLOR 
            {
                //return i.moreMasks.x;
            
                #if NORMALS_ON

                    float3 nrm = normalize(i.vertexColor.xyz);
                    return float4(
                        dot(UNITY_MATRIX_V[0].xyz, nrm) * .5 + .5,
                        dot(UNITY_MATRIX_V[1].xyz, nrm) * .5 + .5,
                        max(0, dot(UNITY_MATRIX_V[2].xyz, nrm)),
                        1);
                #endif

                //return i.matCapWall.w;

                fixed sunLight = i.texCoord4.x;


            //  Sampling Shadow and Light Maps  //
                fixed4 shadows = GetMapA(i.mapUVs.xy);
                fixed4 lights  = GetMapB(i.mapUVs.zw);

                fixed wallShadows = lights.r;
                fixed floorMask   = i.masks.w;

                fixed megaShadowFade = saturate(1 - (abs(i.texCoord5.x) * .24 - .55));
                //return megaShadowFade;
                
                fixed3 skyMix = GetSkyColor(i.texCoord5.yzw);
                
            //  Albedo  //
                #if GREY_ON
                    fixed3 albedo = fixed3(.65, .65, .65);
                #else 
                    fixed3 albedo = i.vertexColor.xyz;
                #endif
                
                albedo = lerp(albedo, skyMix, i.moreMasks.x * .45);


            //  Body Occlusion  //
                fixed playerZMask = i.texCoord4.y;
                      playerZMask = 1 - (playerZMask * playerZMask * .5);
                fixed maskPow     = 1 - shadows.a * i.masks.z * playerZMask;
                fixed maskLerp    = .2 + saturate(maskPow * maskPow * maskPow) * .8;
                

                //return (1 - wallShadow);
            //  Vertex Shade  //
                fixed wallShadow         = saturate(i.texCoord5.x);
                fixed addSunlightInFront = sunLight * .1 * (1 - wallShadow);
                
                //return i.matCapWall.z;
                fixed wallShade = saturate(i.matCapWall.z + addSunlightInFront) * .85 + .15;
                fixed shadePow  = 1 - min( min(wallShade + (1 - wallShadows * (1 - floorMask)), i.vertexColor.a), maskLerp);
                fixed vShade    = .3 + saturate(1 - (shadePow * shadePow * shadePow * shadePow * shadePow)) * .7;


            //  wallFog  //
                fixed wallFade = 1 - saturate(i.fog.w);


            //  Shadows  //
                fixed castShadows       = shadows.r * i.masks.x * CastShadowVis;
                fixed contactShadows    = (shadows.g - .15) * i.masks.y;
                fixed projectionShadows = max(shadows.b, lights.b * wallFade);

                fixed wallCast = wallShadow * wallShadow * wallShadow * wallShadows * WallCastSideMulti;  // .65 + .35 * saturate(-Side)
                fixed megaS    = max(projectionShadows, wallCast) * megaShadowFade  * MegaSideMulti;      // 1 + .1 * saturate(Side);
                //return megaS;
                fixed shadowFade  = (1 - pow(wallFade, 4)) * .6;
                fixed contactCast = max(contactShadows, castShadows * (1 - megaS)) * shadowFade;
                fixed shadowVis   = (1 - contactCast) + i.texCoord4.w; //  noShadows
                
                //return shadowVis;
                fixed shadowMix   = saturate(min(shadowVis, vShade));
                

                megaS *= CastShadowVis;


            //  Direct Light + Extra Light  //
                fixed3 directLight = saturate(sunLight * (1 - megaS * .7) + .2) * shadowMix * SunColor + Ambient;
                //return fixed4(directLight, 1);
                fixed bottomLight = i.texCoord4.z;
                fixed extraLerp   = bottomLight * (shadowMix * shadowMix) * (1 - megaS * (.05 + saturate(-Side) * .4));
                //return extraLerp;
                fixed3 lightMix    = fixed3(1 - (AntiSunColor.r + (1 - AntiSunColor.r) * extraLerp) * directLight.r, 
                                            1 - (AntiSunColor.g + (1 - AntiSunColor.g) * extraLerp) * directLight.g, 
                                            1 - (AntiSunColor.b + (1 - AntiSunColor.b) * extraLerp) * directLight.b);
                                            
                //return float4(1 - lightMix, 1);
              
                #if TEST_ON
                    return extraLerp;
                #endif


            //  MatCap Reflection  //
                fixed3 reflection = GetReflection(i.matCapWall.xy, shadowMix, megaS) * .6;

            //  Result  //
                fixed3 result = albedo - lightMix + (1 - shadowMix) * ShadowColor + reflection;
                       result = GetFogResult(result, skyMix, i.fog.xyz, megaShadowFade, wallFade, floorMask);

                #if DESATURATE_ON
                    return (.3 * result.r + .59 * result.g + .11 * result.b);
                #endif

                //return fixed4(result, 1);
                return float4(pow(saturate(1 - pow(1 - saturate(result), 1.3)), 1.3), 1);
            }
        
            ENDCG
        }
    }
    FallBack "Diffuse"
}
