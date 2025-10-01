#include "UnityCG.cginc"

sampler _MatCap;
sampler _Caustic;


float4 Flattened(float4 vert)
{
    vert.z *= .5; //.75; //.25;
    return vert;
}


fixed2 GetMatCapUV(float3 normal)
{
    fixed2 matCapNormal = mul(UNITY_MATRIX_V, float4(normal, 0)).rg;
                
    return fixed2((matCapNormal.r *  .485) + .5, 
                   matCapNormal.g *  .485 + .5);
}
            
            
float MatCapX(float3 normal)
{
    return tex2D(_MatCap, GetMatCapUV(normal)).x;
}  
            
            
float RefMulti(float3 normal)
{
    return 1 + (MatCapX(normal) - .5) * .5;
}



float _Light, _Shade;


float Light(float3 normal)
{
    float light = saturate(dot(normal, normalize(float3(.2, 1, -.5))));

    return light * _Light - (1 - light) * _Shade;
}


float Cone(float3 worldPos)
{
    float3 dir = _WorldSpaceCameraPos - worldPos;
           dir.z = 0;
           
    float l = 1; //1 + (sin(_Time.y) * .5 + .5) * 10000;
    return (1 - saturate(pow((dir.x * dir.x + dir.y * dir.y) * .00018 * 1.5 * 1.3, 2))) * .75 + .25;
}


float3 hsv2rgb(float3 c) 
{
    c = float3(c.x, clamp(c.yz, 0.0, 1.0));
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


float3 ColorTest(float3 c, float cone)
{
    float l = 1 - cone;
    c.x += l * l * l * -.35 * 2;// + _Time.y;
    //c.y += l * -.4;
    //c.z = saturate(c.z);
    //c.z += l * -.4;
    return hsv2rgb(c);
}

//float3 ColorTest(float3 c, float cone)
//{
//    float l = 1 - cone;
//    c.x += l * -.4;// + _Time.y;
//    c.y += l * -.4;
//    c.z = saturate(c.z);
//    c.z += l * -.4;
//    return hsv2rgb(c);
//}


float CValue(float value, float t)
{
    return value * (pow((sin(t) * .5 + .5), 2));
}



float Caustic(float3 worldPos, float normalY)
{
    normalY = pow(saturate(normalY * .5 + .5), 3) * .9 + .1;
    
    float t = _Time.y * 5;
    float4 sample = tex2D(_Caustic, float2(worldPos.x + t * .02, worldPos.y + t * .55) * .0065);
    float s = 3.14159265 * 2.0 / 4.0;
    return (min(.6, 
           max(CValue(sample.x, t), 
           max(CValue(sample.y, t + s), 
           max(CValue(sample.z, t + s * 2), CValue(sample.w, t + s * 3))))) 
           *.95 + .05) * .7 * normalY;
}