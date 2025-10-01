// https://upload.wikimedia.org/math/2/8/5/2851c9dc2031127e6dacfb84b96446d8.png

#define PI 3.1415926538
float _Radius;
float4x4 _ToWorld;
float _MakeFlat;

struct sphereOutput
{
	float4 pos;
	float3 normal;
};
			

float2 SphereRads(float4 pos)
{
    float circumference = 2 * PI * _Radius;
    
    float zLerp = pos.z * 2 / circumference;
    float zRad  =  2 * PI * zLerp;
    
    float xLerp = pos.x * 2 / circumference;
    float xRad  = -2 * PI * xLerp;
    
    return float2(xRad, zRad);
}
                
            
float3 RotateAroundX (float3 dir, float rad)
{
    float sina, cosa;
    sincos(rad, sina, cosa);
    
    float3x3 m = float3x3(1, 0, 0, 0, cosa, -sina, 0, sina, cosa);
    return mul(m, dir).xyz;
}

float3 RotateAroundY (float3 dir, float rad)
{
    float sina, cosa;
    sincos(rad, sina, cosa);
    
    float3x3 m = float3x3(cosa, 0, sina, 0, 1, 0, -sina, 0, cosa);
    return mul(m, dir).xyz;
}
            
float3 RotateAroundZ (float3 dir, float rad)
{
    float sina, cosa;
    sincos(rad, sina, cosa);
    
    float3x3 m = float3x3(cosa, -sina, 0, sina, cosa, 0, 0, 0, 1);
    return mul(m, dir).xyz;
}


float3 RotateDirByRads(float2 rads, float3 dir)
{
    return normalize(RotateAroundZ(dir, rads.x) + RotateAroundX(dir, rads.y));
}


sphereOutput SphereOutput(float4 pos, float3 normal)
{
    sphereOutput s;
    
    float2 rads = SphereRads(pos);
    
    float4 spherePos    = mul(_ToWorld, float4(RotateDirByRads(rads, float3(0, 1, 0)) * (_Radius + pos.y) + float3(0, - _Radius, 0), 1));
    float3 sphereNormal = RotateDirByRads(rads, normal);
    
    s.pos    = lerp(spherePos, pos, _MakeFlat); 
    s.normal = lerp(sphereNormal, normal, _MakeFlat);
    
    return s;
}

