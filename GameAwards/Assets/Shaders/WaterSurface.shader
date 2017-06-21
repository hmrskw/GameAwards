// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaterSurface"
{
	Properties
	{
		_WaveTex1     ("WaveTex1"    , 2D         ) = "bump" {}
		_WaveTex2     ("WaveTex2"    , 2D         ) = "bump" {}
		
		_WaveTilling  ("WaveTiling"  , Vector     ) = (0, 0, 0, 0)
		
		_Color        ("Color"       , Color      ) = (1, 1, 1, 1)
		_AlbedoColor  ("Albedo Color", Color      ) = (0, 0, 0, 0)
		
		_Glossiness   ("Smoothness"  , Range(0, 1)) = 0.5
		_Reflection   ("Reflection"  , Vector     ) = (0, 0, 0, 0)

		_FlowSpeed    ("FlowSpeed"   , Vector     ) = (0, 0, 0, 0)
	}

	SubShader
	{
		Tags
		{
			"Queue"      = "Transparent"
			"RenderType" = "Transparent"
		}

		GrabPass{}
			CGPROGRAM
			// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata members tangent)
			//#pragma exclude_renderers d3d11

			#pragma target 5.0
			#pragma surface surf Standard 
			#include "UnityCG.cginc"

			sampler2D _GrabTexture;

			sampler2D _WaveTex1;
			sampler2D _WaveTex2;

			fixed4    _AlbedoColor;

			half4     _WaveTilling;
			fixed4    _Color;

			half      _Glossiness;
			half4     _Reflection;

			half4     _FlowSpeed;

			struct appdata {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float4 color  : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
			};

			struct Input
			{
				float2 uv_WaveTex1;
				float4 screenPos;
			};

			void surf (Input IN, inout SurfaceOutputStandard o) {
				fixed4 waveTex1 = tex2D(_WaveTex1, IN.uv_WaveTex1 * _WaveTilling.x + float2(0, _Time.x * _FlowSpeed.x));
				fixed4 waveTex2 = tex2D(_WaveTex2, IN.uv_WaveTex1 * _WaveTilling.y + float2(0, _Time.x * _FlowSpeed.y));

				fixed3 normal1 = UnpackNormal(waveTex1);
				fixed3 normal2 = UnpackNormal(waveTex2);
				fixed3 normal = BlendNormals(normal1, normal2);

				fixed3 distortion1 = UnpackScaleNormal(waveTex1, _Reflection.x);
				fixed3 distortion2 = UnpackScaleNormal(waveTex2, _Reflection.y);
				fixed2 distortion = BlendNormals(distortion1, distortion2).rg;


				half2 grabUV = (IN.screenPos.xy / IN.screenPos.w) * float2(1, -1) + float2(0, 1);
				grabUV.y = grabUV.y * -1 + 1;
				half3 grab = tex2D(_GrabTexture, grabUV + distortion).rgb * _Color;


				o.Albedo     = _AlbedoColor;
				o.Emission   = grab;
				o.Metallic   = 0;
				o.Smoothness = _Glossiness;
				o.Normal     = normal;
				o.Alpha      = 1;
			}

			ENDCG
		}

	FallBack "Transparent/Diffuse"
}
