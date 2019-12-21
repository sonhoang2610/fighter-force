// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Amplify/GrayScale 1"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}

		_Float0("Float 0", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }


		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float4 _ClipArgs0 = float4(1000.0, 1000.0, 0.0, 1.0);
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float2  worldPos : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float4 _MainTex_ST;
			uniform float _Float0;
			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif
				OUT.worldPos = IN.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode2 = tex2D( _MainTex, uv_MainTex );
				float grayscale3 = (tex2DNode2.rgb.r + tex2DNode2.rgb.g + tex2DNode2.rgb.b) / 3;
				float4 temp_cast_1 = (grayscale3).xxxx;
				float4 lerpResult5 = lerp( tex2DNode2 , temp_cast_1 , _Float0);
				
				fixed4 c = lerpResult5;
			c.a = tex2DNode2.a;
			// Softness factor
			float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
			// Apply clipping
			c.a *= clamp(min(factor.x, factor.y), 0.0, 1.0);
			clip(c.a - 0);
				return c;
			}
		ENDCG
		}
	}
	SubShader
	{
	   LOD 100


	   Tags
	   {
		  "Queue" = "Transparent"
		  "IgnoreProjector" = "True"
		  "RenderType" = "Transparent"
	   }

	   Pass
	   {
		  Cull Off
		  Lighting Off
		  ZWrite Off
		  Fog { Mode Off }
		  ColorMask RGB
		  Blend SrcAlpha OneMinusSrcAlpha
		  ColorMaterial AmbientAndDiffuse

		  SetTexture[_MainTex]
		  {
			 Combine Texture * Primary
		  }
	   }
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
2017;29;1816;1004;1114;529;1;True;True
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;1;-1243.2,36.6;Float;False;0;0;_MainTex;Shader;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1032.2,-38.4;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-731.2,164.6;Float;True;Property;_Float0;Float 0;0;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;3;-670.2,-130.4;Float;False;2;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;5;-338.2,-102.4;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;14;Hidden/Templates/Legacy/Sprites Default NGUI 1;074cc0dfde3927848ae7c516e09cd07d;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;3;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;0
WireConnection;2;0;1;0
WireConnection;3;0;2;0
WireConnection;5;0;2;0
WireConnection;5;1;3;0
WireConnection;5;2;4;0
WireConnection;0;0;5;0
WireConnection;0;1;2;4
ASEEND*/
//CHKSM=15CE875B033BD0EF3F592D6287E7E0B787B6B3EC