Shader "Hidden/NewImageEffectShader 2"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
		_Speed("Speed",float) = 10
			_Count("Count",float) = 4
				_Size("Size",float) = 40
			_Noise("Noise",float) = 0.21
		_OffsetX("OffsetX",float) = 0
			_OffsetY("OffsetY",float) = 0
	}
		SubShader
	{
	   Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent"}
	   // No culling or depth
		Cull Off Blend SrcAlpha OneMinusSrcAlpha ZWrite Off ZTest Always

	   Pass
	   {
		   CGPROGRAM
		   #pragma vertex vert
		   #pragma fragment frag

		   #include "UnityCG.cginc"
		   struct appdata
		   {
			   float4 vertex : POSITION;
			   float2 uv : TEXCOORD0;
			   float4 color    : COLOR;
		   };

		   struct v2f
		   {
			   float2 uv : TEXCOORD0;
			   float4 vertex : SV_POSITION;
			   float4 color    : COLOR;
		   };
		   float _OffsetX, _OffsetY;
		   v2f vert(appdata v)
		   {
			   v2f o;
			   o.vertex = UnityObjectToClipPos(v.vertex);
			   o.uv = v.uv;
			   o.uv += float2(_OffsetX, _OffsetY);
			   o.color = v.color;
			   return o;
		   }

		   float Hash(float2 p, in float s)
		   {
			   float3 p2 = float3(p.x,p.y, 27.0 * abs(sin(s)));
			   //    return frac(sin(dot(p2,float3(27.1,61.7, 12.4)))*273758.5453123);
			   return frac(sin(dot(p2, float3(27.1, 61.7, 12.4))) * 2.1);
		   }

		   float _Noise;
		   float noise(in float2 p, in float s)
		   {
			   float2 i = floor(p);
			   float2 f = frac(p);
			   f *= f * (3.0 - 2.0 * f);


			   return lerp(lerp(Hash(i + float2(0., 0.), s), Hash(i + float2(1., 0.), s), f.x),
				   lerp(Hash(i + float2(0., 1.), s), Hash(i + float2(1., 1.), s), f.x),
				   f.y) * s;
		   }


		   float fbm(float2 p)
		   {
			   float v = 0.0;
			   v += noise(p * 2., _Noise);
			   //    v += noise(p*2., 0.25);
			   //    v += noise(p*4., 0.125);
			   //   v += noise(p*8., 0.0625);
			   return v;
		   }
		   float4 TurnBlackToAlpha(float4 txt, float force, float fade)
		   {
			   float3 gs = dot(txt.rgb, float3(1., 1., 1.));
			   gs = saturate(gs);
			   return lerp(txt, float4(force * txt.rgb, gs.r), fade);
		   }
		   sampler2D _MainTex;
		   float _Speed;
		   float _Count;
		   float _Size;

		   fixed4 _Color;
		   fixed4 frag(v2f i) : COLOR
		   {
		 float worktime = _Time.y * _Speed;

   float2 uv = (i.uv.xy) * 2.0 - 1.0;

   uv *= 2.;

   float3 finalColor = float3(0.0, 0.0, 0.0);
   float alpha = 0;
   //明るさ
   for (float i = 1.; i < _Count; ++i)
	   {
	   //        float t = abs(1.0 / ((uv.x + fbm( uv + worktime/i)) * (i*50.0)));
			   float t = abs(1.0 / ((uv.y + fbm(uv + worktime / i)) * (i * _Size)));
			   finalColor += t * float3(i * 0.3 + _Color.x, i * 0.3 + _Color.y, i * 0.3 + _Color.z);
			   alpha += t*t;
		   }
   float4 pColor = TurnBlackToAlpha(float4(finalColor, alpha), 1, 1);
   pColor.a *= alpha;
	   return  pColor;
		   }
		   ENDCG
	   }
	}
}
