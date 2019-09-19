Shader "Spine/Skeleton SpriteMaskable" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		_StencilRef ("Stencil Reference", Float) = 1.0
		_StencilComp ("Stencil Compare", Float) = 4.0 // For float values, see https://docs.unity3d.com/Manual/SL-Stencil.html under "Comparison Function" heading
		_FillColor ("FillColor", Color) = (1,1,1,1)
		_FillPhase ("FillPhase", Range(0, 1)) = 0
		_GrayPhase("Phase", Range(0, 1)) = 1
	}

	SubShader {
		Tags { 	"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True" }

			Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass {
			Stencil {
				Ref [_StencilRef]  //Customize this value
				Comp [_StencilComp] //Customize the compare function
				Pass Keep
			}

			Fog { Mode Off }
			ColorMaterial AmbientAndDiffuse
				CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _FillColor;
			float _FillPhase;
			float _GrayPhase;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv = v.uv;
				o.vertexColor = v.vertexColor;
				o.pos = UnityObjectToClipPos(v.vertex); // Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
				return o;
			}

			float4 frag (VertexOutput i) : COLOR {
				float4 rawColor = tex2D(_MainTex,i.uv);
				float finalAlpha = (rawColor.a * i.vertexColor.a);
				float3 finalColorGray = lerp(rawColor.rgb, dot(rawColor.rgb, float3(0.3, 0.59, 0.11)), _GrayPhase);
				float3 finalColor = lerp((finalColorGray.rgb * i.vertexColor.rgb), (_FillColor.rgb * finalAlpha), _FillPhase); // make sure to PMA _FillColor.
				return fixed4(finalColor, finalAlpha);
			}
			ENDCG
			SetTexture [_MainTex] {
				Combine texture * primary
			}
			
		}


	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Pass {
			ColorMaterial AmbientAndDiffuse
			SetTexture [_MainTex] {
				Combine texture * primary DOUBLE, texture * primary
			}
		}
	}
}
