// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effect/Pulsation"
{
	Properties
	{
		_PulseTex      ("Pulse Texture"         , 2D             ) = "white" {}
		_MaskTexLabel  ("Mask Label Texture"    , 2D             ) = "white" {}
		_MaskTexVessel ("Mask Vessel Texture"   , 2D             ) = "white" {}
		_CutoffBorder  ("Cutoff Border"         , Range(0.0, 1.0)) = 0.1
		_ScrollSpeed   ("Scroll Speed Multiple" , Float          ) = 1.0 
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
				half2  uv2    : TEXCOORD1;
			};

			struct Input 
			{
				float2 uv_MaskTexLabel;
			};

			sampler2D _PulseTex;
			sampler2D _MaskTexLabel;
			sampler2D _MaskTexVessel;
			float4    _PulseTex_ST;
			float4    _MaskTexLabel_ST;
			float     _CutoffBorder;
			float     _ScrollSpeed;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _PulseTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _MaskTexLabel);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				Input IN;
				fixed4 col  = tex2D(_PulseTex     , i.uv);
				fixed4 col2 = tex2D(_MaskTexLabel , i.uv2 + float2(0, _Time.y * _ScrollSpeed));
				fixed4 col3 = tex2D(_MaskTexVessel, i.uv);
				
				fixed alpha;
				
				alpha = col2.r * col3.r;

				return fixed4(col.r, col.g, col.b, alpha);
			}
			ENDCG
		}
	}

	Fallback "Diffuse"
}
