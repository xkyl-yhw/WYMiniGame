// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/DynamicGrass"
{
    Properties
    {
        _GrassColorTop("GrassColorTop", Color) = (1,1,1,1)
		_GrassColorBottom ("GrassColorBottom", Color) = (1,1,1,1)
		_ShadowColor ("ShadowColor", Color) = (1,1,1,1)
		_GradientbTex ("GradientbTex", 2D) = "white" {}
		//hit obstacle
		_EffectTopOffset ("EffectTopOffset", float) = 2
		_EffectBottomOffset ("EffectBottomOffset", float) = -1
		_OffsetGradientStrength ("OffsetGradientStrength", range(0, 1)) = 0.7
		_OffsetFixedRoots ("OffsetFixedRoots", Range(0, 1)) = 1
		_OffsetMultiplier ("OffsetMultiplier", range(0.1, 2)) = 2
		_GravityGradientStrength ("GravityGradientStrength", range(0, 1)) = 0.7
		_GravityFixedRoots ("GravityFixedRoots", range(0, 1)) = 0.7
		_GravityMultiplier ("GravityMultiplier", range(0, 1)) = 0.7

		//wind
		_ShakeWindSpeed ("ShakeWindSpeed", float) = 0
		_WindDirRate ("WindDirRate", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fwdbase_fullshadows
			#include "AutoLight.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD1;
				LIGHTING_COORDS(3,4)
            };

			float4 _GrassColorTop, _GrassColorBottom, _ShadowColor;
            sampler2D _MainTex, _GradientbTex;
            float4 _MainTex_ST, _GradientbTex_ST;
			float _PositionArray;
			float3 _ObstaclePositions[100];
			float _EffectRadius, _BendAmount, _EffectTopOffset, _EffectBottomOffset, _OffsetGradientStrength;
			float _OffsetFixedRoots, _OffsetMultiplier, _GravityGradientStrength, _GravityFixedRoots, _GravityMultiplier;

			float _ShakeDisplacement, _ShakeWindSpeed, _ShakeBending, _WindDirRate;
            float _WindDirectionX, _WindDirectionZ, _WindStrength;

			void FastSinCos (float4 val, out float4 s, out float4 c) 
            {
                val = val * 6.408849 - 3.1415927;
                // powers for taylor series
                float4 r5 = val * val;
                float4 r6 = r5 * r5;
                float4 r7 = r6 * r5;
                float4 r8 = r6 * r5;
                float4 r1 = r5 * val;
                float4 r2 = r1 * r5;
                float4 r3 = r2 * r5;
                //Vectors for taylor's series expansion of sin and cos
                float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841};
                float4 cos8 = {-0.5, 0.041666666, -0.0013888889, 0.000024801587};
                // sin
                s = val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
                // cos
                c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
            }

            v2f vert (appdata v)
            {
                v2f o;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _GradientbTex);

				fixed4 gradientCol = tex2Dlod(_GradientbTex, float4(TRANSFORM_TEX(v.uv, _GradientbTex), 0, 0));
				float gradient = lerp(gradientCol.g, 1, 1 - _OffsetGradientStrength);
				float xzOffset = lerp(o.uv.y * gradient, 1, 1 - _OffsetFixedRoots);

				float gravityCurvature = lerp(gradientCol.g, 1, 1 - _GravityGradientStrength);
				float yOffset = lerp(o.uv.y * gravityCurvature, 1, 1 - _GravityFixedRoots);
				float3 yMultiplier = float3(0, -1, 0) * _GravityMultiplier;
				float2 gravityDistRate = float2(0, 0);

				const float _WindSpeed = _ShakeWindSpeed;

				const float4 _waveXSize = float4 (0.048, 0.06, 0.24, 0.096);
                const float4 _waveZSize = float4 (0.024, 0.08, 0.08, 0.2);
                const float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);

                float4 _waveXmove = float4 (0.024, 0.04, -0.12, 0.096);
                float4 _waveZmove = float4 (0.006, 0.02, -0.02, 0.1);

				float4 waves;

				waves = v.vertex.x * _waveXSize;
				waves += v.vertex.z * _waveZSize;

				waves += _Time.x * waveSpeed * _WindSpeed + v.vertex.x + v.vertex.z;

				float4 s, c;
				waves = frac(waves);
				FastSinCos(waves, s, c);
				float waveAmount = v.uv.y * _ShakeBending;

				s *= waveAmount;
				s *= normalize(waveSpeed);

				float fade = dot(s, 1.3);

                float3 waveMove = float3 (0, 0, 0);

                float windDirX = _WindDirectionX * _WindStrength;
                float windDirZ = _WindDirectionZ * _WindStrength;

                waveMove.x = dot (s, _waveXmove * windDirX);
                waveMove.z = dot (s, _waveZmove * windDirZ);

                float3 windDirOffset = float3 (windDirX * _WindDirRate, 0, windDirZ * _WindDirRate) * xzOffset;

                gravityDistRate += float2 (windDirX, windDirZ);

                float3 waveForce = -mul ((float3x3)unity_WorldToObject, waveMove).xyz * xzOffset + windDirOffset;

                v.vertex.xyz += waveForce;

                for (int n = 0; n < _PositionArray; n++){

                    // get char top and bottom pos as effect range in Y.
                    float charTopY = _ObstaclePositions[n].y + _EffectTopOffset;
                    float charBottomY = _ObstaclePositions[n].y + _EffectBottomOffset;
                    float charY = clamp (o.posWorld.y, charBottomY, charTopY); // when Y is within the Range (charTopY to charBottomY)

                    // get bend force by distance --------------------------
                    float dist = distance (float3(_ObstaclePositions[n].x, charY, _ObstaclePositions[n].z), o.posWorld.xyz); // get distance of char and vertices
                    float effectRate = clamp (_EffectRadius - dist, 0, _EffectRadius); // get bend rate within 0 ~ max
                    float3 bendDir = normalize (o.posWorld.xyz - float3 (_ObstaclePositions[n].x, o.posWorld.y, _ObstaclePositions[n].z)); // get bend dir
                    float3 bendForce = bendDir * effectRate;

                    gravityDistRate += float2 (o.posWorld.x - _ObstaclePositions[n].x, o.posWorld.z - _ObstaclePositions[n].z) * effectRate;

                    // get final bend force
                    float3 finalBendForce = xzOffset * bendForce * _OffsetMultiplier;

                    // set bend force to vertices offset ================================================
                    v.vertex.xyz += finalBendForce;
                }
                float gravityForce = length (gravityDistRate);
                v.vertex.xyz += gravityForce * yMultiplier * yOffset;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                // using fog
                UNITY_TRANSFER_FOG (o, o.pos);

                TRANSFER_VERTEX_TO_FRAGMENT (o); // make light work

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                 // sample the texture
                fixed4 Gradient = tex2D(_GradientbTex, i.uv);
                fixed4 col = lerp (_GrassColorBottom, _GrassColorTop, Gradient.g);

                // apply light and shadow
                float attenuation = LIGHT_ATTENUATION(i);
                fixed4 shadowCol = lerp (_ShadowColor, fixed4 (1,1,1,1), attenuation);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col * shadowCol;
            }
            ENDCG
        }
    }FallBack "VertexLit"
}
