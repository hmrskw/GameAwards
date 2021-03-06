﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effect/Pulsation"
{
	Properties
	{
		_PulseTex      ("Pulse Texture"         , 2D             ) = "white" {}
		_MaskTexVessel ("Mask Vessel Texture"   , 2D             ) = "white" {}
		
		[HideInInspector]
		_InputAlpha    ("Input Alpha"           , Float          ) = 1.0
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
		Cull Off
		ZWrite Off
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata 
			{
				float4 vertex : POSITION;
				float2 uv     : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION; 
				half2  uv     : TEXCOORD0;
			};

			sampler2D _PulseTex;
			sampler2D _MaskTexVessel;
			float4    _PulseTex_ST;
			fixed     _InputAlpha;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _PulseTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col  = tex2D(_PulseTex     , i.uv);
				fixed4 col2 = tex2D(_MaskTexVessel, i.uv);
				
				fixed alpha;
				alpha = col2.r * _InputAlpha;

				return fixed4(col.r, col.g, col.b, alpha);
			}
			ENDCG
		}
	}

	Fallback "Diffuse"
}
