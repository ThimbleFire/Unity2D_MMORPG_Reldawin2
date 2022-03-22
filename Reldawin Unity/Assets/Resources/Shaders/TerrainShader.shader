Shader "LowCloud/TerrainShader"
{
	Properties
	{
		_Textures("Sprite Texture", 2DArray) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_TextureIndex("Texture Index", Int) = 0
		[MaterialToggle] Premultiplied_Alpha("Premultiplied Alpha", Int) = 1
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}
	SubShader
	{
		Tags
		{
			//"Queue" = "Transparent" // Line removed to fix rendering outlines
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		BlendOp Add
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex VertexFunc
			#pragma fragment FragmentFunc
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ PREMULTIPLIED_ALPHA_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   	: POSITION;
				float4 color    	: COLOR;
				float2 uv_bottomLayer   : TEXCOORD0;
				float2 uv_topLayer   	: TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv_bottomLayer  : TEXCOORD0;
				float2 uv_topLayer   : TEXCOORD1;
			};

			fixed4 _Color;

			v2f VertexFunc(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv_bottomLayer = IN.uv_bottomLayer;
				OUT.uv_topLayer = IN.uv_topLayer;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _Textures;
			sampler2D _AlphaTex;
UNITY_SAMPLE_TEX2DARRAY(name,uv)
			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_Textures, uv);

				return color;
			}

			fixed4 FragmentFunc(v2f IN) : SV_Target
			{
				fixed4 background = SampleSpriteTexture(IN.uv_bottomLayer) * _Color;
				fixed4 foreground = SampleSpriteTexture(IN.uv_topLayer) * _Color;

				fixed topAlpha = 1 - foreground.a;
				fixed botAlpha = 1 - background.a;

				if (foreground.a > 0)
				{
					fixed4 finalColor = { 0, 0, 0, 0 };

					#ifdef PREMULTIPLIED_ALPHA_ON
					finalColor.rgb = (foreground.rgb) + (background.rgb * (1 - foreground.a));
					#else
					finalColor.rgb = (foreground.rgb * foreground.a) + (background.rgb * (1 - foreground.a));
					#endif
					//return alpha
					return finalColor;
				}

				//return opaque
				return background;
			}

			ENDCG
		} // end pass
	} //end subshader
} // end
