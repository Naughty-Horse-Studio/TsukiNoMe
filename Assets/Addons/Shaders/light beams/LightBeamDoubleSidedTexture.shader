// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
Shader "Custom/LightBeamDoubleSidedTexture"
{
	Properties
	{
		[HideInInspector]
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_FadeDist("Fade Distance", Float) = 12
		_TimeXInc("Time movement x", Float) = 0.01
		_TimeYInc("Time movement y", Float) = 0.02
		_LerpStart("Lerp start", Float) = -0.5
		_LerpEnd("Lerp end", Float) = 2.5
		_Power("Fade Power",Float) = 2
		_NormalPower("Normal Power", Float) = 1
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }
		LOD 3000


		Pass
	{
		Name "FORWARD"
		Tags{ "LightMode" = "ForwardBase" }

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag

		fixed4 _Color;
		sampler2D _MainTex;
		float _FadeDist;
		float _TimeXInc;
		float _TimeYInc;
		float _LerpStart;
		float _LerpEnd;
		float _Power;
		float _NormalPower;


		struct Input
		{
			float4 pos : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 posWS : TEXCOORD0;
			float3 modelPos : TEXCOORD1;
			float3 normal : TEXCOORD2;
			float2 uv : TEXCOORD3;
		};

		v2f vert(Input In)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(In.pos);

			float4 posWS = mul(unity_ObjectToWorld, In.pos);
			o.posWS = posWS;

			float3 p;
			p.x = unity_ObjectToWorld[0].w;
			p.y = unity_ObjectToWorld[1].w;
			p.z = unity_ObjectToWorld[2].w;
			o.modelPos = p;


			o.normal = mul((float3x3)unity_ObjectToWorld, In.normal);

			o.uv = In.uv;

			return o;
		}

		void frag(v2f In,float face : VFACE, out fixed4 OUT : SV_Target)
		{
			float2 uv = In.uv;
			uv.x += _Time.y * _TimeXInc;
			uv.y -= _Time.y * _TimeYInc;

			fixed4 col = tex2D(_MainTex, uv) * _Color;

			float fadeStart = 0;
			float fadeEnd = _FadeDist;

			float d = length(In.posWS.xyz - In.modelPos);
			float fade = 1 - saturate((d - fadeStart) / (fadeEnd - fadeStart));

			fade = pow(fade, _Power);

			float3 dir2Cam = _WorldSpaceCameraPos.xyz - In.posWS.xyz;
			dir2Cam = normalize(dir2Cam);
			float3 normal = In.normal * sign(face);


			float dotVal = max(0.0, dot(normalize(normal), dir2Cam));
			float val = pow(dotVal, _NormalPower);
			fade *= max(0.0f, lerp(_LerpStart, _LerpEnd, val));

			OUT = fixed4(_Color.rgb + col.rgb, _Color.a * fade);
		}

		ENDCG
		}
		}
		Fallback "Transparent/VertexLit"
		CustomEditor "LightBeamColorEditor"
}
