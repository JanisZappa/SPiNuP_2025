// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "andy/SpineShadow" 
{
    SubShader 
    {
        Pass 
        {
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 2.0

            #define M_PI 3.1415926535897932384626433832795

            uniform fixed3 ShadowDir;

            struct VertexInput 
            {
                float4 vertex      : POSITION;
               
                float2 texcoord0   : TEXCOORD0;
                float2 texcoord1   : TEXCOORD1;
                float2 texcoord2   : TEXCOORD2;
                float3 texcoord3   : TEXCOORD3;

                float4 vertexColor : COLOR;
            };

            struct VertexOutput 
            {
                float4 pos         : SV_POSITION;
                
                float2 uv          : TEXCOORD0;
                float2 uv1         : TEXCOORD1;
                float2 uv2         : TEXCOORD2;
                float2 posWorld    : TEXCOORD3;

                float4 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v) 
            {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor  = v.vertexColor;
                o.posWorld     = mul(unity_ObjectToWorld, v.vertex);
                o.pos          = UnityObjectToClipPos( v.vertex );
               
                o.uv           = v.texcoord0;
                o.uv1          = v.texcoord1;
                o.uv2          = v.texcoord2;

                return o;
            }

            ////////////////// BEGIN QUATERNION FUNCTIONS //////////////////

            float PI = 3.1415926535897932384626433832795;
            
            float4 setAxisAngle (float3 axis, float rad) {
              rad = rad * 0.5;
              float s = sin(rad);
              return float4(s * axis[0], s * axis[1], s * axis[2], cos(rad));
            }

        //  Andy Version  //
            float4 getRot (float rad) 
            {
              rad = rad * .5;
              float s = sin(rad);
              return float4(0, 0, s, cos(rad));
            }
            

            float3 xUnitVec3 = float3(1.0, 0.0, 0.0);
            float3 yUnitVec3 = float3(0.0, 1.0, 0.0);
            
            float4 rotationTo (float3 a, float3 b) {
              float vecDot = dot(a, b);
              float3 tmpvec3 = float3(0, 0, 0);
              if (vecDot < -0.999999) {
                tmpvec3 = cross(xUnitVec3, a);
                if (length(tmpvec3) < 0.000001) {
                  tmpvec3 = cross(yUnitVec3, a);
                }
                tmpvec3 = normalize(tmpvec3);
                return setAxisAngle(tmpvec3, PI);
              } else if (vecDot > 0.999999) {
                return float4(0,0,0,1);
              } else {
                tmpvec3 = cross(a, b);
                float4 _out = float4(tmpvec3[0], tmpvec3[1], tmpvec3[2], 1.0 + vecDot);
                return normalize(_out);
              }
            }
            
            float4 multQuat(float4 q1, float4 q2) {
              return float4(
                q1.w * q2.x + q1.x * q2.w + q1.z * q2.y - q1.y * q2.z,
                q1.w * q2.y + q1.y * q2.w + q1.x * q2.z - q1.z * q2.x,
                q1.w * q2.z + q1.z * q2.w + q1.y * q2.x - q1.x * q2.y,
                q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z
              );
            }
            
            float3 rotateVector( float4 quat, float3 vec ) {
              // https://twistedpairdevelopment.wordpress.com/2013/02/11/rotating-a-vector-by-a-quaternion-in-glsl/
              float4 qv = multQuat( quat, float4(vec, 0.0) );
              return multQuat( qv, float4(-quat.x, -quat.y, -quat.z, quat.w) ).xyz;
            }

        //  Andy Version  //
            float3 rotateV( float rad, float2 vec ) 
            {
              float4 quat = getRot(rad);
              float4 qv = multQuat( quat, float4(vec, 0.0, 0.0) );
              return multQuat( qv, float4(-quat.x, -quat.y, -quat.z, quat.w) ).xyz;
            }


            
            ////////////////// END QUATERNION FUNCTIONS //////////////////


            fixed ringtest(float2 pos, float2 midPoint, float radius)
            {
                return 1 - saturate(saturate(pow(1 - saturate(abs(radius - length(float2(midPoint.x - pos.x, midPoint.y - pos.y)) * .3)), 3)));
            }


            fixed DistToSegment(float2 pos, float2 midPoint, float radius, float height, float thickness, float mainRad)
            {  
                fixed2 pM       = pos - midPoint;
                fixed shadowDot = saturate((dot(pM * .6, normalize(float2(-ShadowDir.x, -ShadowDir.y)))));



                fixed  absR    = abs(radius);
                fixed2 forward = fixed2(sign(-radius), 0);
                fixed2 right   = rotateV( M_PI * .5, forward);

                fixed2 center  = midPoint + fixed2(radius, 0);

                fixed2 nPos = midPoint + rotateV(mainRad, (pos - midPoint));

                fixed2 posDir  = normalize(nPos - center);
                fixed  dirSign = sign(dot(right, posDir));

                fixed  tipRad = (clamp(height - thickness * 2, 0, height)) / (absR * 2 * M_PI) * M_PI * dirSign;
                fixed2 tipDir = rotateV( tipRad, forward);

                fixed minDot = max(dot(posDir, forward), dot(tipDir, forward));
                fixed minRad = acos(minDot) * dirSign;


                fixed2 closestDir = rotateV(minRad, forward);
                fixed2 closest    = center + closestDir * abs(radius);
                fixed2 cToPos     = nPos - closest;

                fixed dist = length(cToPos);

                fixed a = 1 - pow(1 - (saturate((dist - thickness) * .14 )), 15);
                fixed b = 1 - pow(1 - (saturate((dist - thickness) * .16 )), 7);

                return saturate(lerp(a, b, shadowDot) + length(pM) * .04); // + saturate((1- pow(saturate(1 - dist * 4), 2))) * .2;
               
            }


            float4 frag(VertexOutput i) : COLOR 
            {
                fixed shadowLerp = DistToSegment(i.posWorld, i.uv, i.uv1.r, i.uv2.r, i.uv2.g, i.uv1.g);

                float3 emissive = lerp(i.vertexColor.rgb, float3(0, 0, 0), shadowLerp);
                return fixed4(emissive, 1);
            }    
            ENDCG
        }
    }
}
