Shader "MissileCommand/ControllableGlitchUrp"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

	HLSLINCLUDE

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	TEXTURE2D_X(_MainTex);
	SAMPLER(sampler_MainTex);
	half _Amount;
	half _Frame;
	half2 _Pixel;
	half4 _MainTex_TexelSize;

	struct AppData
	{
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct V2F
	{
		half4 pos : POSITION;
		half4 uv : TEXCOORD0;
#ifdef PIXEL
		half4 uv1 : TEXCOORD1;
#endif
		UNITY_VERTEX_OUTPUT_STEREO
	};

	half r(half2 v)
	{
		return frac(sin(dot(v * floor(_Time.y * 12.0h), half2(127.1h, 311.7h))) * 43758.5453123h);
	}

	V2F vert(AppData i)
	{
		V2F o = (V2F)0;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, half4(i.pos.xyz, 1.0h)));
		o.uv.xy = UnityStereoTransformScreenSpaceTex(i.uv);
		o.uv.zw = half2(o.uv.x + _Amount, o.uv.x - _Amount);
#ifdef PIXEL
		o.uv1 = floor(half4(i.uv.xy * half2(24.0h, 9.0h), i.uv.xy * half2(8.0h, 4.0h)));
#endif
		return o;
	}

	half4 frag(V2F i) : COLOR
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
#ifdef FRAME
        // Bunch of maths to make nice wibbles. Different scales and frequencies to make noisy shapes. 
        // ^ No input noise texture though, so no random involved. 
        // _Time is time since level load; float4: t/20, t, t*2, t*3
        // i.uv.y allows us to modify the value dependent on the y position, i.uv.x for the x position
        // i.uv.xwz isn't documented well, but looks like it allows us to move the x value without affecting the y. 
        // ^ Probably related to float4 jazz that I don't really grok atm.
		i.uv.xwz += _Frame * (
            sin(_Time.w * 10.0h + i.uv.y * 125.0h + i.uv.x * i.uv.x) * 0.25h + 
            sin(_Time.w * 10.0h + i.uv.y * i.uv.y * 500.0h) * 0.15h + 
            sin(_Time.w * 5.0h + i.uv.y * 10.0h + i.uv.x * 10.0h) * 3.0h + 
            cos(_Time.w * 2.0h + i.uv.y * 5.0h) * 2.0h
        );
#endif
#ifdef PIXEL
		half lineNoise = pow(r(floor(i.uv1.xy)), 8.0) * pow(r(floor(i.uv1.zw)), 3.0);
		i.uv.x += lineNoise * _Pixel.x;
		i.uv.w -= lineNoise * _Pixel.y;
#endif
		return half4(SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, i.uv.zy).r, SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, i.uv.xy).g, SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, i.uv.wy).b, 1.0h);
	}
	ENDHLSL

	Subshader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			HLSLPROGRAM
			#pragma shader_feature_local FRAME
			#pragma shader_feature_local PIXEL
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDHLSL
		}
	}
	Fallback off
}