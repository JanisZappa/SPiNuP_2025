Shader "Andy/Sdf"{
    SubShader{
        Tags{ "RenderType"="Transparent" "Queue"="Transparent"}

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

        Pass{
            CGPROGRAM

            #include "UnityCG.cginc"
            #include "sdfmath.cginc"

            #pragma vertex vert
            #pragma fragment frag

            struct appdata{
                float4 vertex : POSITION;
            };

            struct v2f{
                float4 position : SV_POSITION;
                float4 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v){
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float wobble(float radius, float offset)
            {
            	return (sin(_Time.y * 8 + offset) * .125 + .125 + .75) * radius;
            }

            float scene(float2 position) 
            {
                float a = circle(translate(position, float2(sin(_Time.y), 0)), wobble(2, 0));
                float b = circle(translate(position, float2(sin(_Time.y * 1.5 + 1) * 3, 0)), wobble(1, 5));
				float c = circle(translate(position, float2(0, sin(_Time.y * 2.5 + 1) * 4)), wobble(1, 21));
                return round_merge(round_merge(a, b, .5), c, .5);
            }

            fixed4 frag(v2f i) : SV_TARGET{
                float dist = scene(i.worldPos.xy);

				float distanceChange = fwidth(dist) * 0.5;
   				float antialiasedCutoff = smoothstep(distanceChange, -distanceChange, dist);
    		    fixed4 col = fixed4(float3(1, 1, 0), antialiasedCutoff);
    		    return col;
            }

            ENDCG
        }
    }
}
