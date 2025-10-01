// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "andy/MainMatSetColor" 
{
    Properties 
    {
        [KeywordEnum(Off, On)] USETEX ("TexToggle", Float) = 0
        MainTex ("MainTex", 2D) = "white" {}
    }

    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0

            #pragma multi_compile SIMPLE_OFF     SIMPLE_ON
            #pragma multi_compile GREY_OFF       GREY_ON
            #pragma multi_compile DESATURATE_OFF DESATURATE_ON
            #pragma multi_compile USETEX_OFF     USETEX_ON
            
            #pragma multi_compile DEV_OFF        DEV_ON
            #pragma multi_compile TEST_OFF       TEST_ON

            uniform sampler2D MainTex;   uniform float4 MainTex_ST;
            uniform sampler2D Palette;   uniform float4 Palette_ST;
            uniform sampler2D ShadowTex; uniform float4 ShadowTex_ST;
            uniform sampler2D LightTex;  uniform float4 LightTex_ST;
            uniform sampler2D MatCap;    uniform float4 MatCap_ST;


            uniform fixed Side;
            uniform fixed SideMulti;

            uniform fixed SideM1;
            uniform fixed SideM2;

            uniform fixed Wall;
            uniform fixed GamePlane;

            uniform fixed3 SunAngle;
            uniform fixed2 SunFactors;
            uniform fixed  SunPowLerp;

        //  Colors  //
            uniform fixed3 SunColor;
            uniform fixed3 AntiSunColor;
            uniform fixed3 Bounce;
            uniform fixed3 FogColor;
            uniform fixed3 SkyColor;
            uniform fixed3 SunSkyMixColor;
            uniform fixed3 Ambient;
            uniform fixed3 ShadowA;
            uniform fixed3 ShadowB;
            
            uniform fixed3 SunAngleSteep;
            uniform fixed3 ShadowDir;
            uniform fixed  ContactAdd;
            uniform fixed  ContactMultiFactor;

            uniform fixed  ShadowVisibility;

            uniform fixed4 MapA;
            uniform fixed4 MapB;

            uniform fixed  PaletteFactor;
            uniform fixed  PaletteOffset;

            uniform fixed  MatCapXMulti;

            uniform fixed3 SkySize;
            uniform fixed3 SkyCenter;

            uniform fixed  TexPan;

            uniform fixed ViewDirY;
            

            struct VertexInput 
            {
                float4 vertex      : POSITION;
                float3 normal      : NORMAL;
                float4 vertexColor : COLOR;
            };


            struct VertexOutput
            {
                fixed4 vertexColor : COLOR;

                fixed4 texCoord0 : TEXCOORD0;
                fixed4 texCoord1 : TEXCOORD1;
                fixed4 texCoord2 : TEXCOORD2;
                fixed4 texCoord3 : TEXCOORD3;
                fixed4 texCoord4 : TEXCOORD4;
                float4 texCoord5 : TEXCOORD5;
                fixed4 texCoord6 : TEXCOORD6;
            };


            VertexOutput vert (VertexInput v, out float4 outpos : SV_POSITION) 
            {
                VertexOutput o = (VertexOutput)0;
                outpos = UnityObjectToClipPos(v.vertex);


            //  Simple  //
                #if SIMPLE_ON
                    #if USETEX_ON
                        o.texCoord0 = fixed4(.25, .25, .25, 1);
                    #else    
                        o.texCoord0 = fixed4(tex2Dlod(Palette, float4(v.vertexColor.r * 255 * PaletteFactor + PaletteOffset, PaletteOffset, 0, 0))); 
                    #endif
        
                    return o;
                #endif


            //  Default  //
                float3 normal   = UnityObjectToWorldNormal(v.normal);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float4 color    = v.vertexColor;


            //  Color //
                #if GREY_ON ||  USETEX_ON
                    fixed3 albedo = fixed3(.65, .65, .65);
                #else
                    fixed3 albedo = fixed3(.6, .3 ,0);//tex2Dlod(Palette, float4(1 * 255 * PaletteFactor + PaletteOffset, PaletteOffset, 0, 0)).xyz; 
                #endif


            //  Shadow Masks  //
                  int  maskCode = color.b * 255;
                fixed    item_M = 0;
                fixed contact_M = 0;
                fixed  player_M = 0;
                fixed   floor_M = 0; //min(maskCode & (1 << 3), 1);
             

             //  UVs  //
                fixed2 tex_UV  = fixed2(worldPos.x, worldPos.y + TexPan);

                fixed offsetFactor = (worldPos.z - Wall * Side) / MapA.w * 5;
                fixed2 offset = SunFactors * offsetFactor;
                fixed2 mapPos = worldPos + offset;
                fixed2 mapA_UV = fixed2((mapPos.x - MapA.x) * MapA.z, (mapPos.y - MapA.y) * MapA.z);
                fixed2 mapB_UV = fixed2((mapPos.x - MapB.x) * MapB.z, (mapPos.y - MapB.y) * MapB.z);


            //  MatCap UV  //
                fixed2 matCapNormal = mul(UNITY_MATRIX_V, float4(normal, 0)).rg;
                fixed  matCapPick   = 1 * 255 * MatCapXMulti;
                fixed  matCapY      = matCapNormal.g * .5 + .5;

                fixed2 matCapUV     = fixed2((matCapNormal.r * -.5  * Side + .5) * MatCapXMulti + matCapPick, 
                                              matCapNormal.g * .5 + .5);

            //  Fog UV  //
                fixed2 fogUV  = fixed2((worldPos.r - SkyCenter.r) / SkySize.r, (worldPos.b - SkyCenter.b) / SkySize.b);


            //  Dot Products  //
                fixed sunDot    = saturate(dot(SunAngle,      normal));
                fixed sun2Dot   = saturate(dot(SunAngleSteep, normal) *   .5 + 1.25);
                fixed shadowDot = saturate(dot(ShadowDir,     normal) * -1.5 +  .05);


            //  WallStuff  //
                fixed absPosZ = abs(worldPos.z);

                fixed wallLerp     = (Wall - worldPos.z) / (Wall * 2);
                fixed wallSideLerp = SideMulti - wallLerp * Side;

                fixed shadeMulti = (.35 + .65 * sunDot) * ContactMultiFactor;
                fixed wallShade  = (1 + ((ContactAdd + (absPosZ - Wall) * shadeMulti) + abs(normal.z) * .075 - 1) * (1 - contact_M)) * 1.15;

                fixed wallBounce = (absPosZ - Wall) * .25 * (1 - contact_M);

                fixed bodyPow = GamePlane - absPosZ;

                float3 worldDir = worldPos.xyz - _WorldSpaceCameraPos;
              

            //  Result  //
                o.vertexColor = fixed4(albedo, color.a);

                o.texCoord0 = fixed4(tex_UV, wallShade, sunDot);
                o.texCoord1 = fixed4(sun2Dot, shadowDot, fogUV);
                o.texCoord2 = fixed4(mapA_UV, mapB_UV);
                o.texCoord3 = fixed4(item_M, contact_M, player_M, floor_M);
                o.texCoord4 = fixed4(wallSideLerp, bodyPow, matCapUV);
                o.texCoord5 = float4(1 - wallLerp, worldDir);
                o.texCoord6 = fixed4((worldPos.g - SkyCenter.g), wallBounce, 0, 0);
  
                return o;
            }


            float4 frag(VertexOutput i, UNITY_VPOS_TYPE vpos : VPOS) : COLOR 
            {

            //  Simple  //
                #if SIMPLE_ON
                    return i.texCoord0;
                #endif


                fixed2 tex_UV    = i.texCoord0.xy;
                fixed  sunDot    = i.texCoord0.w;

                fixed  sun2Dot   = i.texCoord1.x;
                fixed  shadowDot = i.texCoord1.y;
                fixed2 fogUV     = fixed2(i.texCoord1.z / (1 + abs(i.texCoord1.w * 5)), i.texCoord1.w);

                fixed2 mapA_UV = i.texCoord2.xy;
                fixed2 mapB_UV = i.texCoord2.zw;

                fixed    item_M = 0;
                fixed contact_M = 0;
                fixed  player_M = 0; 
                fixed   floor_M = 0;

                fixed  wallPow  = 1 - saturate(i.texCoord4.x);
                fixed  bodyPow  = i.texCoord4.y;
                fixed2 matCapUV = i.texCoord4.zw;

                fixed  wallShadow = saturate(i.texCoord5.x);
                float3 worldDir   = normalize(i.texCoord5.yzw);

                fixed wallMega  = clamp(i.texCoord5.x * .16, -1, 1);
                      wallMega *= wallMega;
                      wallMega *= wallMega;
                      wallMega  = 1 - wallMega;
                      wallMega *= wallMega;
                      wallMega  = 1 - wallMega;


                fixed wallShade = saturate(i.texCoord0.z + sunDot * .25 * (1 - wallShadow));

                fixed wallBounce = saturate(i.texCoord6.g * i.texCoord6.g);

                //return sunDot;
                //  Albedo  //
                    #if USETEX_ON && !GREY_ON
                        fixed3 albedo = tex2D(MainTex, TRANSFORM_TEX( tex_UV, MainTex));
                    #else
                        fixed3 albedo = i.vertexColor.xyz;
                    #endif

                    //return fixed4(albedo, 0);


                //  Sampling Shadow Tex  //
                    fixed4 shadows = tex2D(ShadowTex, TRANSFORM_TEX(mapA_UV, ShadowTex));
                    fixed4 lights  = tex2D(LightTex,  TRANSFORM_TEX(mapB_UV, LightTex));


                    fixed wallCast = wallShadow * wallShadow * wallShadow * lights.a;
                    fixed megaS    = max(shadows.b, wallCast) * (1 - floor_M + floor_M * (1 - wallMega)) * ShadowVisibility;

                //  Body Occlusion  //
                    fixed bodyMask = 1 - (bodyPow * bodyPow * .5);
                    fixed maskPow  = 1 - shadows.a * bodyMask * player_M;
                    fixed maskLerp = .15 + min(maskPow * maskPow * maskPow, 1) * .85;

                    //return maskLerp;


                //  Vertex Shade  //
                    fixed shadePow = 1 - min(min(wallShade + (1 - lights.a * (1 - floor_M)), i.vertexColor.a), maskLerp);
                    fixed vShade   = saturate(1 - (shadePow * shadePow * shadePow * shadePow * shadePow));
                    

                //  MaskedWall
                    fixed maskedWall = wallPow * (1 - floor_M);

                //  Shadows  //
                    fixed shadowFade  = 1 - wallPow;
                          shadowFade *= shadowFade;
                          shadowFade *= shadowFade;
                          shadowFade *= shadowFade;

                    fixed  merged_rb = max((shadows.g - .05) * contact_M, shadows.r * item_M * (1 - megaS * SideM1) * ShadowVisibility) * shadowFade * .7;
                    fixed  shadowVis = (1 - merged_rb * .7) + shadowDot;
                    fixed  shadowMix = saturate(min(shadowVis, vShade));
                    fixed3 addShadow = (1 - shadowMix) * .2 * lerp(ShadowB, ShadowA, shadowMix);
                    

                //  Direct Light + Extra Light  //
                    fixed3 directLight = saturate(sunDot * .9 * (1 - megaS * .7) + .25) * shadowMix * SunColor + Ambient;
                    fixed  extraLerp   = sun2Dot * shadowMix * (1 - megaS * SideM2);
                    fixed3 lightMix    = fixed3(1 - (AntiSunColor.r + (1 - AntiSunColor.r) * extraLerp) * directLight.r, 
                                                1 - (AntiSunColor.g + (1 - AntiSunColor.g) * extraLerp) * directLight.g, 
                                                1 - (AntiSunColor.b + (1 - AntiSunColor.b) * extraLerp) * directLight.b);


                //  MatCap Reflection  //
                    fixed4 matCapMix = tex2D(MatCap, TRANSFORM_TEX(matCapUV, MatCap));
                    fixed  tint      = matCapMix.a;

                    fixed  matFactor    = shadowMix * shadowMix * shadowMix * (1 - shadows.b * .5);
                    fixed  mirrorFactor = matCapMix.r * matCapMix.r * matFactor;
                    fixed3 reflection   = fixed3(mirrorFactor * (SunSkyMixColor.r + (albedo.r - SunSkyMixColor.r) * tint), 
                                                 mirrorFactor * (SunSkyMixColor.g + (albedo.g - SunSkyMixColor.g) * tint), 
                                                 mirrorFactor * (SunSkyMixColor.b + (albedo.b - SunSkyMixColor.b) * tint));


                //  Sky Gradient + Fog  //
                    fixed  gradientPow = saturate(1 - worldDir.y * 1.666 + .4);

                    fixed3 skyGradient = lerp(FogColor, SkyColor, 1 - (gradientPow * gradientPow * gradientPow));
                    fixed  dirPow  = saturate(dot(SunAngle, worldDir));
                           dirPow  = dirPow + (dirPow * dirPow * dirPow - dirPow) * SunPowLerp;
                          // dirPow *= dirPow;

                    fixed3 sunSkyDot = dirPow * .8 * SunColor;
                    //return dirPow + sunDot * .1;

                    fixed fogY = abs(i.texCoord6.x * (.0054 + .0085 * (1 - wallMega)));
                    //return fogY;
                    //return saturate(fogY * fogY) * .25;


                    fixed3 skyMix  = skyGradient + sunSkyDot;
                    fixed  fogLerp = max(max(wallPow * .175, min(fogUV.x * fogUV.x + fogUV.y * fogUV.y, 1)), saturate(fogY * fogY * .4));
                    //return fogLerp;


                //  LightTex  //
                    fixed3 light    = (1 - shadows.b * .5) * lights.rgb;
                    fixed  itemL    = saturate(1 - (wallBounce + (1 - vShade))) * (1 - contact_M); 
                    fixed3 lightAdd = (itemL + contact_M * (1 - wallPow)) * light;


                //  Split Up Albedo + Lighting  //
                //  And all the other Fog Effects  //
                //  Result  //
                    fixed3 result = lerp(albedo - lightMix + addShadow + maskedWall * .25 * skyMix + reflection + lightAdd, skyMix, fogLerp);

                //  Desaturate  //
                    #if DESATURATE_ON
                        return (.3 * result.r + .59 * result.g + .11 * result.b);
                    #endif


                //  Developmemnt Help  //
                    #if DEV_ON
                        return fixed4(fixed3(1 - (AntiSunColor.r + (1 - AntiSunColor.r) * extraLerp), 
                                             1 - (AntiSunColor.g + (1 - AntiSunColor.g) * extraLerp), 
                                             1 - (AntiSunColor.b + (1 - AntiSunColor.b) * extraLerp)), 1);
                    #endif
                    #if TEST_ON
                        return extraLerp;
                    #endif

                return fixed4(result, 1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
