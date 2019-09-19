//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/trailfire"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
RotationUV_Rotation_1("RotationUV_Rotation_1", Range(-360, 360)) = 270
RotationUV_Rotation_PosX_1("RotationUV_Rotation_PosX_1", Range(-1, 2)) = 0.5
RotationUV_Rotation_PosY_1("RotationUV_Rotation_PosY_1", Range(-1, 2)) =0.5
RotationUV_Rotation_Speed_1("RotationUV_Rotation_Speed_1", Range(-8, 8)) =0
_Generate_Fire_PosX_2("_Generate_Fire_PosX_2", Range(-1, 2)) = 0
_Generate_Fire_PosY_2("_Generate_Fire_PosY_2", Range(-1, 2)) = 0
_Generate_Fire_Precision_2("_Generate_Fire_Precision_2", Range(0, 1)) = 0.038
_Generate_Fire_Smooth_2("_Generate_Fire_Smooth_2", Range(0, 1)) = 0.5
_Generate_Fire_Speed_2("_Generate_Fire_Speed_2",float) = 2
_Palette4Swapping_Color1_1("_Palette4Swapping_Color1_1", COLOR) = (1,0.8593202,0,1)
_Palette4Swapping_Color2_1("_Palette4Swapping_Color2_1", COLOR) = (1,0.8341393,0,1)
_Palette4Swapping_Color3_1("_Palette4Swapping_Color3_1", COLOR) = (1,0.6744844,0,1)
_Palette4Swapping_Color4_1("_Palette4Swapping_Color4_1", COLOR) = (1,0.6192256,0,1)
ZoomUV_Zoom_1("ZoomUV_Zoom_1", Range(0.2, 4)) = 1.128
ZoomUV_PosX_1("ZoomUV_PosX_1", Range(-3, 3)) = 0.5
ZoomUV_PosY_1("ZoomUV_PosY_1", Range(-3, 3)) =-3
_Generate_Fire_PosX_1("_Generate_Fire_PosX_1", Range(-1, 2)) = 0
_Generate_Fire_PosY_1("_Generate_Fire_PosY_1", Range(-1, 2)) = 0
_Generate_Fire_Precision_1("_Generate_Fire_Precision_1", Range(0, 1)) = 0.05
_Generate_Fire_Smooth_1("_Generate_Fire_Smooth_1", Range(0, 1)) = 0.5
_Generate_Fire_Speed_1("_Generate_Fire_Speed_1", Range(-2, 2)) = 1
_Add_Fade_1("_Add_Fade_1", Range(0, 4)) = 1
_CircleHole_PosX_1("_CircleHole_PosX_1", Range(-1, 2)) = -0.373
_CircleHole_PosY_1("_CircleHole_PosY_1", Range(-1, 2)) = 0.5
_CircleHole_Size_1("_CircleHole_Size_1", Range(0, 1)) = 0.371
_CircleHole_Dist_1("_CircleHole_Dist_1", Range(0, 1)) = 0.257
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off 

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float RotationUV_Rotation_1;
float RotationUV_Rotation_PosX_1;
float RotationUV_Rotation_PosY_1;
float RotationUV_Rotation_Speed_1;
float _Generate_Fire_PosX_2;
float _Generate_Fire_PosY_2;
float _Generate_Fire_Precision_2;
float _Generate_Fire_Smooth_2;
float _Generate_Fire_Speed_2;
float4 _Palette4Swapping_Color1_1;
float4 _Palette4Swapping_Color2_1;
float4 _Palette4Swapping_Color3_1;
float4 _Palette4Swapping_Color4_1;
float ZoomUV_Zoom_1;
float ZoomUV_PosX_1;
float ZoomUV_PosY_1;
float _Generate_Fire_PosX_1;
float _Generate_Fire_PosY_1;
float _Generate_Fire_Precision_1;
float _Generate_Fire_Smooth_1;
float _Generate_Fire_Speed_1;
float _Add_Fade_1;
float _CircleHole_PosX_1;
float _CircleHole_PosY_1;
float _CircleHole_Size_1;
float _CircleHole_Dist_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 RotationUV(float2 uv, float rot, float posx, float posy, float speed)
{
rot=rot+(_Time*speed*360);
uv = uv - float2(posx, posy);
float angle = rot * 0.01744444;
float sinX = sin(angle);
float cosX = cos(angle);
float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
uv = mul(uv, rotationMatrix) + float2(posx, posy);
return uv;
}
float2 ZoomUV(float2 uv, float zoom, float posx, float posy)
{
float2 center = float2(posx, posy);
uv -= center;
uv = uv * zoom;
uv += center;
return uv;
}
float Generate_Fire_hash2D(float2 x)
{
return frac(sin(dot(x, float2(13.454, 7.405)))*12.3043);
}

float Generate_Fire_voronoi2D(float2 uv, float precision)
{
float2 fl = floor(uv);
float2 fr = frac(uv);
float res = 1.0;
for (int j = -1; j <= 1; j++)
{
for (int i = -1; i <= 1; i++)
{
float2 p = float2(i, j);
float h = Generate_Fire_hash2D(fl + p);
float2 vp = p - fr + h;
float d = dot(vp, vp);
res += 1.0 / pow(d, 8.0);
}
}
return pow(1.0 / res, precision);
}

float4 Generate_Fire(float2 uv, float posX, float posY, float precision, float smooth, float speed, float black)
{
uv += float2(posX, posY);
float t = _Time*60*speed;
float up0 = Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(0, -t), precision);
float up1 = 0.5 + Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(42, -t ) + 30.0, precision);
float finalMask = up0 * up1  + (1.0 - uv.y);
finalMask += (1.0 - uv.y)* 0.5;
finalMask *= 0.7 - abs(uv.x - 0.5);
float4 result = smoothstep(smooth, 0.95, finalMask);
result.a = saturate(result.a + black);
return result;
}
float4 Circle_Hole(float4 txt, float2 uv, float posX, float posY, float Size, float Smooth)
{
float2 center = float2(posX, posY);
float dist = 1.0 - smoothstep(Size, Size + Smooth, 1-length(center - uv));
txt.a *= dist;
return txt;
}
float4 Palette4Swap(float4 txt, float2 uv, float4 col1, float4 col2, float4 col3, float4 col4)
{
float4 c1 = col1;
if (txt.r>0.25) c1 = col2;
if (txt.r>0.50) c1 = col3;
if (txt.r>0.75) c1 = col4;
c1.a = txt.a;
return c1;
}
float4 frag (v2f i) : COLOR
{
float2 RotationUV_1 = RotationUV(i.texcoord,RotationUV_Rotation_1,RotationUV_Rotation_PosX_1,RotationUV_Rotation_PosY_1,RotationUV_Rotation_Speed_1);
float4 _Generate_Fire_2 = Generate_Fire(RotationUV_1,_Generate_Fire_PosX_2,_Generate_Fire_PosY_2,_Generate_Fire_Precision_2,_Generate_Fire_Smooth_2,_Generate_Fire_Speed_2,0);
float4 _Palette4Swapping_1 = Palette4Swap(_Generate_Fire_2,i.texcoord,_Palette4Swapping_Color1_1,_Palette4Swapping_Color2_1,_Palette4Swapping_Color3_1,_Palette4Swapping_Color4_1);
float2 ZoomUV_1 = ZoomUV(RotationUV_1,ZoomUV_Zoom_1,ZoomUV_PosX_1,ZoomUV_PosY_1);
float4 _Generate_Fire_1 = Generate_Fire(ZoomUV_1,_Generate_Fire_PosX_1,_Generate_Fire_PosY_1,_Generate_Fire_Precision_1,_Generate_Fire_Smooth_1,_Generate_Fire_Speed_1,0);
_Palette4Swapping_1 = lerp(_Palette4Swapping_1,_Palette4Swapping_1*_Palette4Swapping_1.a + _Generate_Fire_1*_Generate_Fire_1.a,_Add_Fade_1);
float4 _CircleHole_1 = Circle_Hole(_Palette4Swapping_1,i.texcoord,_CircleHole_PosX_1,_CircleHole_PosY_1,_CircleHole_Size_1,_CircleHole_Dist_1);
float4 FinalResult = _CircleHole_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
