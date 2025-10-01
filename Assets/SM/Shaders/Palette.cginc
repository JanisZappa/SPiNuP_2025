//  PRECALC  //
uniform fixed Side;
uniform fixed Wall;
uniform fixed GamePlane;



//  SUN  //
uniform fixed3 SunAngle;
uniform fixed3 SunColor;
uniform fixed  SunPowLerp;

//  Precalc | .75 + (1 - SunPowLerp) * 1.25  //
uniform fixed BottomLightBounceMulti;

//  Precalc | saturate((1 - SunPowLerp) * .2)  //
uniform fixed HeightLightAdd;


//  COLOR + MATCAP ID  //
float2 CM_ID(float4 color)
{
    float matcapValue  = round(color.g * 256);
    float matCapOffset = floor(matcapValue / 16);
    float matCap       = matcapValue - matCapOffset * 16;

    float texID = round(color.r * 256) + matCapOffset * 256;
    
    return float2(texID, matCap);
}



//  PALETTE  //
uniform sampler1D Palette; 
uniform fixed PaletteFactor;
uniform fixed PaletteOffset;        
            
fixed3 GetColor(float id)
{
    return tex1Dlod(Palette, float4(id * PaletteFactor + PaletteOffset, 0, 0, 0)).xyz;
}




//  MATCAPS  //
uniform sampler2D MatCap;
uniform fixed MatCapXMulti;
uniform fixed ReflectionMulti;

fixed2 GetMatCapUV(float id, float3 normal)
{
    fixed2 matCapNormal = mul(UNITY_MATRIX_V, float4(normal, 0)).rg;
               
    #if GREY_ON
    fixed matCapPick = 0 * MatCapXMulti;
    #else 
    fixed matCapPick = id * MatCapXMulti;
    #endif
                
    return fixed2((matCapNormal.r * -.485 * Side + .5) * MatCapXMulti + matCapPick, 
                   matCapNormal.g *  .485 + .5);
}

float3 GetMatCap(fixed2 uv)
{
    return tex2D(MatCap, uv).rgb;
}


fixed3 GetReflection(fixed2 uv, float shadows, float megaShadow)
{
    fixed3 matCapMix  = GetMatCap(uv); // matCapUV
    fixed  matFactor  = shadows * shadows * shadows * (1 - megaShadow * .6) * (.75 + .25 * SunPowLerp) * .45 -.1;
    fixed  refValue   = min((matCapMix.r - .5) * .5, matFactor) * ReflectionMulti * .65;
    
    return fixed3(refValue, refValue, refValue);
}




//  MASKS | Cast Contact Player Floor //
struct Masks
{
    fixed cast;
    fixed contact;
    fixed occlusion;
    fixed floor;
    fixed skyRef;
    
    fixed4 Get()
    {
        return fixed4(cast, contact, occlusion, floor);
    }
    
    fixed4 GetMore()
    {
        return fixed4(skyRef, 0, 0, 0);
    }
};


Masks GetMasks(float3 color, float playerMaskMulti)
{
    int  maskCode  = round(color.b * 256);
    
    Masks m;
    m.cast      = maskCode & (1 << 0);
    m.contact   = saturate(maskCode & (1 << 1));
    m.occlusion = saturate(maskCode & (1 << 2)) * playerMaskMulti * .7;
    m.floor     = saturate(maskCode & (1 << 3));
    
    m.skyRef    = saturate(maskCode & (1 << 4));
    
    return m;
}




//  MAPS  //
uniform fixed4 MapA;
uniform fixed4 MapB;
uniform fixed2 SunFactors;
uniform sampler2D ShadowTex;
uniform sampler2D LightTex;

fixed4 GetMapUVs(float3 worldPos)
{
    fixed offsetFactor = (worldPos.z - Wall * Side) / MapA.w * 5;
    fixed2 mapPos  = fixed2(SunFactors.x * offsetFactor + worldPos.x, SunFactors.y * offsetFactor + worldPos.y);
    
    return fixed4((mapPos.x - MapA.x) * MapA.z, (mapPos.y - MapA.y) * MapA.z,
                  (mapPos.x - MapB.x) * MapB.z, (mapPos.y - MapB.y) * MapB.z);
}

float4 GetMapA(fixed2 uv)
{
    return tex2D(ShadowTex, uv);
}

float4 GetMapB(fixed2 uv)
{
    return tex2D(LightTex, uv);
}




//  FOG  //
uniform fixed3 FogColor;
uniform fixed3 SkyColor;
uniform fixed3 SkyCenter;
uniform fixed3 SkySize;
            
fixed3 GetFogUV(float4 worldPos)
{
    return fixed3((worldPos.r - SkyCenter.r) / SkySize.r,
                  (worldPos.g - SkyCenter.g),
                  (worldPos.b - SkyCenter.b) / SkySize.b);
}

fixed3 GetSkyColor(half3 worldDir)
{
    worldDir = normalize(worldDir);
    fixed  gradientPow = saturate(1 - worldDir.y * 1.666 + .4);

    fixed3 skyGradient = lerp(FogColor, SkyColor, 1 - (gradientPow * gradientPow * gradientPow));
    fixed  dirPow = saturate(dot(SunAngle, worldDir));
           dirPow = dirPow + (dirPow * dirPow * dirPow - dirPow) * SunPowLerp;
           
    return skyGradient + dirPow * .8 * SunColor;
}

fixed3 GetFogResult(fixed3 colors, fixed3 skyMix, fixed3 fogUV, fixed megaShadowFade, fixed wallFade, fixed floorMask)
{
//  The Background is getting less influence of the YFog  | 'wallShadowFade'  //
    fixed fogY = abs(fogUV.y * (.0054 + .0085) * megaShadowFade);
          
    fixed2 uv      = fixed2(fogUV.x / (1 + abs(fogUV.z * 5)), fogUV.z);
    fixed  fogLerp = max(max(wallFade * .175, saturate(uv.x * uv.x + uv.y * uv.y)), saturate(fogY * fogY * .4));
    fixed  wallFog = wallFade * (1 - floorMask) * .25 * skyMix;
                
    return lerp(colors + wallFog, skyMix, fogLerp);
}


