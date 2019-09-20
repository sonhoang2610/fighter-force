// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ParticleClipBounds"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_ClipRanges("ClipRanges", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" /*ase_tags*/ }

		
		SubShader
		{
				Blend SrcAlpha One
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			
			Pass {
			
				CGPROGRAM
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				

				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;

					fixed4 color : COLOR;

					float4 texcoord0 : TEXCOORD0;
					float4 texcoord1 : TEXCOORD1;

					UNITY_VERTEX_INPUT_INSTANCE_ID

					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;

					fixed4 color : COLOR;

					float4 texcoord0 : TEXCOORD0;
					float4 texcoord1 : TEXCOORD1;


					float4 projPos : TEXCOORD3;
				};
				
				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform sampler2D_float _CameraDepthTexture;
				uniform float _InvFade;

				uniform float4 _ClipRanges;

				v2f vert ( appdata_t v  )
				{
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.projPos.xyz = ase_worldPos;
					
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.projPos.w = 0;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);

					#ifdef SOFTPARTICLES_ON

						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);

					#endif

					o.color = v.color;
					
					o.texcoord0 = v.texcoord0;
					o.texcoord1 = v.texcoord1;

					o.texcoord0.xy = TRANSFORM_TEX(v.texcoord0,_MainTex);

					UNITY_TRANSFER_FOG(o,o.vertex);

					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					#ifdef SOFTPARTICLES_ON

						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;

						float fade = saturate (_InvFade * (sceneZ-partZ));

						i.color.a *= fade;

					#endif

					float2 uv_MainTex = i.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float4 tex2DNode3 = tex2D( _MainTex, uv_MainTex );
					float4 break4 = ( 2.0 * _TintColor * i.color * tex2DNode3 );
					float3 ase_worldPos = i.projPos.xyz;
					float2 appendResult39 = (float2(ase_worldPos.x , ase_worldPos.y));
					float2 break6_g1 = appendResult39;
					float4 Rect7 = _ClipRanges;
					float4 break15 = Rect7;
					float2 appendResult16 = (float2(break15.x , break15.y));
					float2 appendResult17 = (float2(break15.z , break15.w));
					float2 Min19 = ( appendResult16 - appendResult17 );
					float2 break4_g1 = Min19;
					float4 break9 = Rect7;
					float2 appendResult10 = (float2(break9.x , break9.y));
					float2 appendResult11 = (float2(break9.z , break9.w));
					float2 Max13 = ( appendResult10 + appendResult11 );
					float2 break7_g1 = Max13;
					float lerpResult44 = lerp( ( ( (( break6_g1.x >= break4_g1.x && break6_g1.x <= break7_g1.x ) ? 1.0 :  0.0 ) * (( break6_g1.y >= break4_g1.y && break6_g1.y <= break7_g1.y ) ? 1.0 :  0.0 ) ) * ( tex2DNode3.a * 1.5 ) ) , 0.0 , ( 1.0 - _TintColor.a ));
					float lerpResult45 = lerp( lerpResult44 , 0.0 , ( 1.0 - i.color.a ));
					float4 break22 = Rect7;
					float2 appendResult25 = (float2(break22.x , break22.y));
					float2 break24 = Max13;
					float2 appendResult28 = (float2(break24.x , break24.y));
					float2 clampResult32 = clamp( (float2( 0,0 ) + (abs( ( appendResult39 - appendResult25 ) ) - ( appendResult28 - float2( 0.5,0.5 ) )) * (float2( 1,1 ) - float2( 0,0 )) / (appendResult28 - ( appendResult28 - float2( 0.5,0.5 ) ))) , float2( 0,0 ) , float2( 1,1 ) );
					float2 break34 = clampResult32;
					float lerpResult38 = lerp( lerpResult45 , 0.0 , max( break34.x , break34.y ));
					float4 appendResult5 = (float4(break4.r , break4.g , break4.b , lerpResult38));
					

					fixed4 col = appendResult5;

					UNITY_APPLY_FOG(i.fogCoord, col);

					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
2017;29;1816;1004;932.7098;599.159;1.026164;True;True
Node;AmplifyShaderEditor.Vector4Node;6;-2372.12,-680.3822;Float;False;Property;_ClipRanges;ClipRanges;0;0;Create;True;0;0;False;0;0,0,0,0;0,0,2,2;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1863.072,-676.2628;Float;False;Rect;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;8;-2610.661,26.82413;Float;False;7;Rect;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;9;-2329.665,28.14844;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1937.651,5.148438;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;-1926.25,164.3482;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-1694.918,69.78444;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-2418.946,-332.0744;Float;False;7;Rect;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1361.596,68.14793;Float;False;Max;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-1702.318,977.0777;Float;False;7;Rect;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;15;-2137.948,-330.7501;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;22;-1499.318,982.0777;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;23;-1515.344,625.1369;Float;False;13;Max;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;21;-1307.501,308.1053;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;17;-1740.936,-189.7502;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;-1745.936,-353.7501;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;25;-1062.725,981.5414;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-1004.085,330.7117;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;18;-1528.24,-271.2431;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;24;-1270.319,685.0778;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;28;-998.7249,710.5414;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;27;-824.94,627.8862;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;26;-1276.55,837.262;Float;False;Constant;_SoftClip;SoftClip;1;0;Create;True;0;0;False;0;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1294.728,-274.0448;Float;False;Min;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;1;-1321.531,-70.2225;Float;False;0;0;_MainTex;Shader;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;-777.3178,830.0777;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;3;-1046.025,-23.82613;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;29;-657.318,631.0777;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-1510.265,518.1833;Float;False;19;Min;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;31;-444.3181,681.0778;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;35;-782.1451,287.4865;Float;False;CompareWithRangeVector2;-1;;1;f9d1d39793953a242b9ca5d99d8c0f3e;0;3;5;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-618.6073,109.3367;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;40;-705.5077,-447.5012;Float;False;0;0;_TintColor;Shader;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;46;-420.6073,-194.6633;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;41;-795.2305,-254.1341;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;32;-284.7253,660.5415;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-369.6168,231.7795;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-504.7993,-521.1705;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;44;-186.6997,145.1867;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;47;-440.6073,-108.6633;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;34;-331.1253,488.3416;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-144.5349,-334.753;Float;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;37;-21.27589,533.9461;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;45;-0.6072998,160.3367;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;100.6817,322.0778;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;4;0.3597412,3.531555;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;5;300.3597,40.53156;Float;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;846.7769,-60.89311;Float;False;True;2;Float;ASEMaterialInspector;0;12;ParticleClipBounds;39608c403216c2545817cb3fb95a1410;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;False;False;False;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;7;0;6;0
WireConnection;9;0;8;0
WireConnection;10;0;9;0
WireConnection;10;1;9;1
WireConnection;11;0;9;2
WireConnection;11;1;9;3
WireConnection;12;0;10;0
WireConnection;12;1;11;0
WireConnection;13;0;12;0
WireConnection;15;0;14;0
WireConnection;22;0;20;0
WireConnection;17;0;15;2
WireConnection;17;1;15;3
WireConnection;16;0;15;0
WireConnection;16;1;15;1
WireConnection;25;0;22;0
WireConnection;25;1;22;1
WireConnection;39;0;21;1
WireConnection;39;1;21;2
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;24;0;23;0
WireConnection;28;0;24;0
WireConnection;28;1;24;1
WireConnection;27;0;39;0
WireConnection;27;1;25;0
WireConnection;19;0;18;0
WireConnection;30;0;28;0
WireConnection;30;1;26;0
WireConnection;3;0;1;0
WireConnection;29;0;27;0
WireConnection;31;0;29;0
WireConnection;31;1;30;0
WireConnection;31;2;28;0
WireConnection;35;5;39;0
WireConnection;35;1;33;0
WireConnection;35;2;23;0
WireConnection;48;0;3;4
WireConnection;46;0;40;4
WireConnection;32;0;31;0
WireConnection;36;0;35;0
WireConnection;36;1;48;0
WireConnection;44;0;36;0
WireConnection;44;2;46;0
WireConnection;47;0;41;4
WireConnection;34;0;32;0
WireConnection;43;0;50;0
WireConnection;43;1;40;0
WireConnection;43;2;41;0
WireConnection;43;3;3;0
WireConnection;37;0;34;0
WireConnection;37;1;34;1
WireConnection;45;0;44;0
WireConnection;45;2;47;0
WireConnection;38;0;45;0
WireConnection;38;2;37;0
WireConnection;4;0;43;0
WireConnection;5;0;4;0
WireConnection;5;1;4;1
WireConnection;5;2;4;2
WireConnection;5;3;38;0
WireConnection;0;0;5;0
ASEEND*/
//CHKSM=AFCF595D5D16859250E6DE90E16261E906F6432F