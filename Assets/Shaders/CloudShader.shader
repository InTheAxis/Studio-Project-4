Shader "Unlit/Clouds"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		  Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		 ZWrite Off
		 Blend SrcAlpha OneMinusSrcAlpha
		// No culling or depth
		// Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// vertex input: position, UV
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewVector : TEXCOORD1;
			};

			v2f vert(appdata v) {
				v2f output;
				output.pos = UnityObjectToClipPos(v.vertex);
				output.uv = v.uv;
				// Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
				// (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
				float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
				output.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
				return output;
			}

		  float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 rayDir) {
			  float3 t0 = (boundsMin - rayOrigin) / rayDir;
			  float3 t1 = (boundsMax - rayOrigin) / rayDir;
			  float3 tmin = min(t0, t1);
			  float3 tmax = max(t0, t1);

			  float dstA = max(max(tmin.x, tmin.y), tmin.z);
			  float dstB = min(tmax.x, min(tmax.y, tmax.z));

			  float dstToBox = max(0, dstA);
			  float dstInsideBox = max(0, dstB - dstToBox);
			  return float2(dstToBox, dstInsideBox);
		  }

		  float4 params;

		  sampler2D _MainTex;
		  sampler2D _CameraDepthTexture;

		  Texture3D<float4> ShapeNoise;
		  Texture3D<float4> DetailNoise;
		  SamplerState samplerShapeNoise;
		  SamplerState samplerDetailNoise;

		  float3 BoundsMin;
		  float3 BoundsMax;

		  int NumSteps;

		  float3 CloudOffset;
		  float CloudScale;
		  float DensityThreshold;
		  float DensityMultiplier;

		  float3 CloudOffsetB;
		  float CloudScaleB;
		  float DensityThresholdB;
		  float DensityMultiplierB;

		  float sampleDensity(float3 position) {
			  float3 uvw = position * CloudScale * 0.001 + CloudOffset * 0.01;
			  float4 shape = ShapeNoise.SampleLevel(samplerShapeNoise, uvw, 0);
			  float density = max(0, shape.r - DensityThreshold) * DensityMultiplier;
			  return density;
		  }
		  float sampleDensityB(float3 position) {
			  float3 uvw = position * CloudScaleB * 0.001 + CloudOffsetB * 0.01;
			  float4 shape = DetailNoise.SampleLevel(samplerDetailNoise, uvw, 0);
			  float density = max(0, shape.r - DensityThresholdB) * DensityMultiplierB;
			  return density;
		  }

		  float4 frag(v2f i) : SV_Target
		  {
			  //return 1;
				float4 col = tex2D(_MainTex, i.uv);
			   float3 rayOrigin = _WorldSpaceCameraPos;
			   float3 rayDir = normalize(i.viewVector);
			   //
			   float nonLinearDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
			   float depth = LinearEyeDepth(nonLinearDepth) * length(i.viewVector);
			   //col *= depth / 100;
			   //col.a = 1;
			   //return col;
			   //

				   float2 rayBoxInfo = rayBoxDst(BoundsMin, BoundsMax, rayOrigin, rayDir);
				   float dstToBox = rayBoxInfo.x;
				   float dstInsideBox = rayBoxInfo.y;
				   //
				   //bool rayHitBox = dstInsideBox > 0;
				   bool rayHitBox = dstInsideBox > 0 && dstToBox < depth;
				   if (!rayHitBox)
					   discard;
				   //

				   float dstTravelled = 0;
				   float stepSize = dstInsideBox / NumSteps;
				   float dstLimit = dstInsideBox;
				   // march
				   float totalDensity = 0;
				   while (dstTravelled < dstLimit) {
					   float3 rayPos = rayOrigin + rayDir * (dstToBox + dstTravelled);
					   totalDensity += sampleDensity(rayPos) * stepSize;
					   totalDensity += sampleDensityB(rayPos) * stepSize;
					   dstTravelled += stepSize;
				   }

				   float transmittance = exp(-totalDensity);
				   //return col * 0.2f + (1 - transmittance);
				   col.a = 1 - (transmittance + (1 - transmittance) * 0.2f);
				   //return col * transmittance + (1 - transmittance);
				   if (col.a < 0.05)
					   discard;

					return col;
			  }

			  ENDCG
		  }
	}
}